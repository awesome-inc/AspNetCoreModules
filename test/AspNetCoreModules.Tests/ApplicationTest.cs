using System;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace AspNetCoreModules.Tests;

public class ApplicationTest : IClassFixture<CustomWebApplicationFactory<SampleApp.Program>>
{
    private readonly CustomWebApplicationFactory<SampleApp.Program> _factory;

    public ApplicationTest(CustomWebApplicationFactory<SampleApp.Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [Theory]
    [InlineData("/api/health/startup", MediaTypeNames.Text.Plain)]
    [InlineData("/api/health/liveness", MediaTypeNames.Text.Plain)]
    [InlineData("/api/health/readiness", MediaTypeNames.Text.Plain)]
    [InlineData("/api/health")]
    [InlineData("/api/blogs")]
    // ReSharper disable once InconsistentNaming
    public async Task ShouldServe(string url, string? mediaType = MediaTypeNames.Application.Json)
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        response.Content.Headers.ContentType!.MediaType.Should().Be(mediaType);
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
