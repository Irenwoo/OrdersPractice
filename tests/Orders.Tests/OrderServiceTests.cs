using Moq;
using Orders.Abstractions;
using Orders.Models;
using Orders.Services;
using Xunit;

namespace Orders.Tests;

public class OrderServiceTests
{
    readonly Mock<IOrderRepository> _repo = new();
    readonly Mock<IMessagePublisher> _pub = new();
    readonly OrderService _sut;

    public OrderServiceTests() =>
        _sut = new(_repo.Object, _pub.Object, new DiscountCalculator());

    static NewOrder Cmd() =>
        new(new List<OrderItem> { new("Книга", 300m, 2) });

    // Тест №10: проверяем, что метод сохранения вызвался ровно 1 раз
    [Fact]
    public async Task CreateAsync_ValidOrder_SavesOrder()
    {
        // Act
        await _sut.CreateAsync(Cmd());

        // Assert: Times.Once() значит "ровно один раз"
        _repo.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Once());
    }

    // Тест №11: проверяем, что опубликовано событие с тем же Id
    [Fact]
    public async Task CreateAsync_ValidOrder_PublishesEventWithSameId()
    {
        // Act
        var id = await _sut.CreateAsync(Cmd());

        // Assert: проверяем, что событие опубликовано, и Id в нем совпадает
        _pub.Verify(
            p => p.PublishAsync(It.Is<OrderCreated>(e => e.OrderId == id)),
            Times.Once());
    }

    // Тест №12: если сохранение упало с ошибкой, событие НЕ должно публиковаться
    [Fact]
    public async Task CreateAsync_SaveFails_DoesNotPublish()
    {
        // Arrange: настраиваем мок, чтобы он выбросил исключение при сохранении
        _repo.Setup(r => r.SaveAsync(It.IsAny<Order>()))
             .ThrowsAsync(new TimeoutException());

        // Act & Assert: ожидаем, что метод выбросит TimeoutException
        await Assert.ThrowsAsync<TimeoutException>(() => _sut.CreateAsync(Cmd()));

        // Assert: проверяем, что публикации НЕ было (Times.Never)
        _pub.Verify(p => p.PublishAsync(It.IsAny<OrderCreated>()), Times.Never());
    }
}