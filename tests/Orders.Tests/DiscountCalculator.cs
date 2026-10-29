using Orders.Services;
using Xunit;

namespace Orders.Tests;

public class DiscountCalculatorTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(999.99, 0)]
    [InlineData(1000, 100)]
    [InlineData(2000, 200)]
    public void Calculate_ReturnsExpected(decimal total, decimal expected)
    {
        // Arrange
        var sut = new DiscountCalculator();

        // Act
        var discount = sut.Calculate(total);

        // Assert
        Assert.Equal(expected, discount);
    }

    [Fact]
    public void Calculate_NegativeTotal_Throws()
    {
        // Arrange
        var sut = new DiscountCalculator();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.Calculate(-1m));
    }
}