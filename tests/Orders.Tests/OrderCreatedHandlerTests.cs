using Moq;
using Orders.Abstractions;
using Orders.Handlers;
using Orders.Models;
using Xunit;

namespace Orders.Tests;

public class OrderCreatedHandlerTests
{
    readonly Mock<INotificationSender> _sender = new();
    readonly OrderCreatedHandler _sut;

    public OrderCreatedHandlerTests() => _sut = new(_sender.Object);

    // Тест №19: проверяем, что при валидном сообщении уведомление отправилось
    [Fact]
    public async Task HandleAsync_ValidMessage_SendsNotification()
    {
        // Arrange
        var id = Guid.NewGuid();
        var message = new OrderCreated(id);

        // Act
        await _sut.HandleAsync(message);

        // Assert: проверяем, что SendAsync вызван 1 раз, и текст содержит Id
        _sender.Verify(s => s.SendAsync(It.Is<string>(text => text.Contains(id.ToString()))),
                       Times.Once());
    }

    // Тест №20: пустой Guid должен вызывать ошибку и НЕ отправлять уведомление
    [Fact]
    public async Task HandleAsync_EmptyOrderId_Throws()
    {
        // Arrange
        var message = new OrderCreated(Guid.Empty);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.HandleAsync(message));

        Assert.Contains("OrderId", ex.Message);
        _sender.Verify(s => s.SendAsync(It.IsAny<string>()), Times.Never());
    }

    // Тест №21: передача null вместо сообщения
    [Fact]
    public async Task HandleAsync_NullMessage_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.HandleAsync(null!));
    }
}