namespace Autofac.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public class OrderAttribute : Attribute
{
    public OrderAttribute(short order)
    {
        Order = order;
    }

    public const short HighestPrecedence = short.MinValue;
    public const short LowestPrecedence = short.MaxValue;
    public short Order { get; }
}
