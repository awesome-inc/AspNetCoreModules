using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[UnsetVisualStudioEnvironmentVariables]
// ReSharper disable once CheckNamespace
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Test);
    

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitVersion] readonly GitVersion GitVersion;
    [Parameter("Is CI Build")] readonly bool IsCiBuild = Host is GitHubActions;
    const string Framework = "net6.0"; //Solution.Projects.First().GetTargetFrameworks()?.First() ?? "net6.0";

    [Parameter("Push built NuGet package")]
    readonly bool IsPushTag = (Environment.GetEnvironmentVariable("GITHUB_REF") ?? "-unset-").StartsWith("refs/tags/");

    [Parameter("NuGet API Key")] readonly string NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
    [Parameter("NuGet Source")] readonly string NuGetSource = "https://www.nuget.org";

    [Parameter("The SonarQube login token")]
    readonly string SonarLogin = Environment.GetEnvironmentVariable("SONAR_LOGIN");

    [Parameter("The SonarQube server")]
    readonly string SonarServer = IsLocalBuild ? "http://localhost:9000" : "https://sonarcloud.io";

    [Parameter("The SonarQube organization")] readonly string SonarOrganization = "awesome-inc";


    static readonly AbsolutePath SourceDirectory = RootDirectory / "src";
    static readonly AbsolutePath TestsDirectory = RootDirectory / "tests";
    static readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    //-------------------------------------------------------------
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj", "**/TestResults").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    //-------------------------------------------------------------
    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(settings => settings
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetFramework(Framework)
                .SetDataCollector("XPlat Code Coverage")
            );
        });

    Target Coverage => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            ReportGeneratorTasks.ReportGenerator(settings => settings
                .SetFramework(Framework)
                .SetReports("**/coverage.cobertura.xml")
                .SetReportTypes(ReportTypes.Cobertura, ReportTypes.SonarQube, ReportTypes.Html)
                .SetTargetDirectory(".coverage/")
            );
        });
    
    //-------------------------------------------------------------
    // cf.: https://github.com/nuke-build/nuke/issues/377#issuecomment-595276623
    const string SonarFramework = "net5.0";

    Target Sonar => _ => _
        .Description("SonarQube analysis")
        .DependsOn(SonarBegin)
        .DependsOn(Coverage)
        .DependsOn(SonarEnd)
        .Executes(() =>
        {
        });

    Target SonarBegin => _ => _
        .Before(Test)
        .Executes(() =>
        {
            SonarScannerTasks.SonarScannerBegin(settings =>
            {
                var name = Solution.Name;
                var key = $"{SonarOrganization}.{name}";
                
                return settings
                    .SetProjectKey(key)
                    .SetName(name)
                    .SetVersion(GitVersion.FullSemVer)
                    .SetVSTestReports("**/*.trx")
                    // cf.: https://github.com/nuke-build/nuke/issues/377#issuecomment-595276623
                    .SetFramework(SonarFramework) //Framework)
                    .SetLogin(SonarLogin) // TODO: should be secret -> SetArgument
                    .SetServer(SonarServer)
                    .SetProcessArgumentConfigurator(args =>
                        //.SetArgumentConfigurator(args =>
                    {
                        // monorepo hack: tell Sonar to scan sub-project only
                        args.Add($"/d:sonar.projectBaseDir={RootDirectory}");
                        // generic coverage data, cf.: https://docs.sonarqube.org/latest/analysis/generic-test/
                        args.Add("/d:sonar.coverageReportPaths=.coverage/SonarQube.xml");
                        if (!string.IsNullOrWhiteSpace(SonarLogin))
                        {
                            // set organization & branch name, cf.:
                            // - https://github.com/nuke-build/nuke/issues/304#issuecomment-522250591
                            // - http://www.nuke.build/docs/authoring-builds/cli-tools.html#custom-arguments
                            args.Add($"/o:{SonarOrganization}");
                            if (GitVersion.BranchName != "main")
                                args.Add($"/d:sonar.branch.name={GitVersion.BranchName}");
                        }

                        return args;
                    });
            });
        });

    Target SonarEnd => _ => _
        .After(Coverage)
        .Executes(() =>
        {
            SonarScannerTasks.SonarScannerEnd(settings => settings
                    .SetLogin(SonarLogin) // TODO: should be secret -> SetArgument
                    // cf.: https://github.com/nuke-build/nuke/issues/377#issuecomment-595276623
                    .SetFramework(SonarFramework)
            );
        });
    
    //-------------------------------------------------------------
    Target Package => _ =>
    {
        return _
            .DependsOn(Test)
            .Produces(ArtifactsDirectory / "*.nupkg")
            .Executes(() =>
            {
                DotNetPack(settings => settings
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetConfiguration(Configuration)
                    .SetVersion(GitVersion.NuGetVersion)
                    .SetNoWarns(5105)
                );
            });
    };

    Target Push => _ => _
        .DependsOn(Package)
        .Executes(() =>
        {
            DotNetNuGetPush(settings => settings
                .SetTargetPath(ArtifactsDirectory / "*.nupkg")
                .SetApiKey(NuGetApiKey)
                .SetSource(NuGetSource)
            );
        });
 
    //-------------------------------------------------------------
    // ReSharper disable once UnusedMember.Local
    Target CiBuild => _ => _
        .Description("CI build target")
        .DependsOn(IsPushTag ? new []{Sonar,Push}: new []{Test})
        .Executes(() =>
        {
        });
}
