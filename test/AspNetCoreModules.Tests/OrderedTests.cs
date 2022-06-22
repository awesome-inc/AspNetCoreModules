using System.Diagnostics.CodeAnalysis;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Xunit;

namespace AspNetCoreModules.Tests;

public class OrderedTests
{
    [Fact]
    public void ShouldOrder()
    {
        var builder = WebApplication.CreateBuilder();
        var hook = new ModuleAppHookBuilder(builder);
        var m1 = new Module1();
        var m2 = new Module2();
        var m3 = new Module3();
        var m4 = new Module4();
        var m5 = new Module5();
        var input = new IAspNetCoreModule[] {m1, m2, m3, m4, m5};
        var actual = hook.Sorted(input);
        actual.Should().ContainInOrder(m2, m5, m4, m3, m1);

        int order = 0;
        hook.Ordered(_ => order++);
        actual = hook.Sorted(input);
        actual.Should().ContainInOrder(m1, m2, m3, m4, m5);
    }
    
    [Order(OrderAttribute.LowestPrecedence)] private class Module1 : AspNetCoreModule {}
    [Order(OrderAttribute.HighestPrecedence)] private class Module2 : AspNetCoreModule {}
    [Order(100)] private class Module3 : AspNetCoreModule {}
    private class Module4 : AspNetCoreModule {}
    private class Module5 : AspNetCoreModule, IHaveOrder
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static")] public short Order => -100;
    }
}
