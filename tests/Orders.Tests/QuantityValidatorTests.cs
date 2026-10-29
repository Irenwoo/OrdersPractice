using Orders.Validation;
using Xunit;

namespace Orders.Tests;

public class QuantityValidatorTests
{
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void IsValidQty_ChecksBounds(int qty, bool expected)
    {
        // Arrange - ничего не создаем, так как класс статический

        // Act
        var actual = QuantityValidator.IsValidQty(qty); // Вызов через имя класса!

        // Assert
        Assert.Equal(expected, actual);
    }
}