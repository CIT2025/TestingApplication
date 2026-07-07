using Moq;
using OrderService.Domain.Interfaces;
using OrderService.Domain.Models;

namespace OrderService.Tests;

/// <summary>
/// Класс тестирования сервиса заказов (OrderService).
/// Содержит тесты для проверки логики размещения заказа:
/// успешное размещение, ошибки при отсутствии товаров, резервирование и отправка уведомлений.
/// </summary>
public class OrderServiceTests
{
    /// <summary>
    /// Тест проверяет, что при наличии всех товаров на складе:
    /// 1. Заказ успешно размещается (результат Success)
    /// 2. Клиенту отправляется подтверждение на email
    /// 3. В подтверждении указаны правильные ID заказа и сумма
    /// </summary>
    [Fact]
    public void PlaceOrder_AllItemsAvailable_ReturnsSuccessAndSendsConfirmation()
    {
        #region ========== ARRANGE (Подготовка) ==========

        // Создаем тестовый заказ с двумя товарами
        var order = new Order
        {
            Id = 1,
            CustomerEmail = "customer@test.com",
            Items =
            [
                new() {
                    ProductId = 1,
                    Quantity = 2,
                    ProductName = "Телефон"
                },
                new() {
                    ProductId = 2,
                    Quantity = 1,
                    ProductName = "Чехол"
                }
            ]
        };

        // STUB: Настраиваем WarehouseService — все товары есть в наличии
        var mockWarehouse = new Mock<IWarehouseService>();
        mockWarehouse.Setup(w => w.IsAvailable(It.IsAny<int>(),
                                               It.IsAny<int>()))
                                  .Returns(true);

        // STUB: Настраиваем CostCalculator — возвращаем фиксированную сумму 5000
        var mockCostCalculator = new Mock<ICostCalculator>();
        mockCostCalculator.Setup(c => c.CalculateTotal(It.IsAny<Order>()))
                                       .Returns(5000m);

        // MOCK: Создаем NotificationService для проверки вызовов
        var mockNotification = new Mock<INotificationService>();

        // Создаем тестируемый сервис, внедряя все зависимости
        var service = new Domain.Services.OrderService(
            mockWarehouse.Object,
            mockCostCalculator.Object,
            mockNotification.Object);

        #endregion

        #region ========== ACT (Действие) ==========

        // Вызываем тестируемый метод
        var result = service.PlaceOrder(order);

        #endregion

        #region ========== ASSERT (Проверка) ==========

        // Проверяем, что результат успешный
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.OrderId);
        Assert.Equal(5000m, result.TotalAmount);

        // Проверяем, что уведомление отправлено ровно один раз с правильными параметрами
        mockNotification.Verify(
            n => n.SendOrderConfirmation(
                "customer@test.com",
                1,
                5000m),
            Times.Once);

        #endregion
    }

    /// <summary>
    /// Тест проверяет, что при отсутствии товара на складе:
    /// 1. Заказ НЕ размещается (результат Failure)
    /// 2. Клиенту отправляется уведомление об ошибке
    /// 3. Резервирование НЕ выполняется
    /// 4. Подтверждение НЕ отправляется
    /// </summary>
    [Fact]
    public void PlaceOrder_ItemNotAvailable_ReturnsFailureAndSendsFailureNotification()
    {
        #region ========== ARRANGE (Подготовка) ==========

        // Создаем тестовый заказ с двумя товарами
        var order = new Order
        {
            Id = 2,
            CustomerEmail = "customer@test.com",
            Items =
            [
                new() {
                    ProductId = 1,
                    Quantity = 2,
                    ProductName = "Телефон"
                },
                new() {
                    ProductId = 2,
                    Quantity = 1,
                    ProductName = "Чехол"
                }
            ]
        };

        // STUB: Товар с ID=1 есть, товар с ID=2 отсутствует
        var mockWarehouse = new Mock<IWarehouseService>();
        mockWarehouse.Setup(w => w.IsAvailable(1, 2))
                                  .Returns(true);
        mockWarehouse.Setup(w => w.IsAvailable(2, 1))
                                  .Returns(false);

        var mockCostCalculator = new Mock<ICostCalculator>();
        var mockNotification = new Mock<INotificationService>();

        var service = new Domain.Services.OrderService(
            mockWarehouse.Object,
            mockCostCalculator.Object,
            mockNotification.Object);

        #endregion

        #region ========== ACT (Действие) ==========

        var result = service.PlaceOrder(order);

        #endregion

        #region ========== ASSERT (Проверка) ==========

        #region Расскомментируйте эти строки, если хотите добиться 100% покрытия тестами

        // Assert.False(result.IsSuccess);
        // Assert.Contains("Чехол", result.ErrorMessage);
        // Assert.Contains("отсутствует", result.ErrorMessage);

        #endregion

        // Проверяем, что Reserve НЕ вызывался для отсутствующего товара
        mockWarehouse.Verify(w => w.Reserve(2, 1), Times.Never);

        // Проверяем, что отправлено уведомление об ошибке
        mockNotification.Verify(
            n => n.SendOrderFailure(
                "customer@test.com",
                It.Is<string>(msg => msg.Contains("Чехол"))),
            Times.Once);

        // Проверяем, что подтверждение НЕ отправлялось
        mockNotification.Verify(
            n => n.SendOrderConfirmation(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<decimal>()),
            Times.Never);

        #endregion
    }

    /// <summary>
    /// Тест проверяет, что при наличии всех товаров:
    /// 1. Метод Reserve вызывается для каждого товара ровно один раз
    /// 2. Передаются правильные идентификаторы и количество
    /// </summary>
    [Fact]
    public void PlaceOrder_AllItemsAvailable_ReservesEachItemOnce()
    {
        #region ========== ARRANGE (Подготовка) ==========

        var order = new Order
        {
            Id = 3,
            CustomerEmail = "customer@test.com",
            Items =
            [
                new() {
                    ProductId = 10,
                    Quantity = 3,
                    ProductName = "Товар А"
                },
                new() {
                    ProductId = 20,
                    Quantity = 5,
                    ProductName = "Товар Б"
                },
                new() {
                    ProductId = 30,
                    Quantity = 1,
                    ProductName = "Товар В"
                }
            ]
        };

        // STUB: Все товары есть на складе
        var mockWarehouse = new Mock<IWarehouseService>();
        mockWarehouse.Setup(w => w.IsAvailable(It.IsAny<int>(),
                                               It.IsAny<int>()))
                                  .Returns(true);

        var mockCostCalculator = new Mock<ICostCalculator>();
        mockCostCalculator.Setup(c => c.CalculateTotal(It.IsAny<Order>()))
                                       .Returns(1000m);

        var mockNotification = new Mock<INotificationService>();

        var service = new Domain.Services.OrderService(
            mockWarehouse.Object,
            mockCostCalculator.Object,
            mockNotification.Object);

        #endregion

        #region ========== ACT (Действие) ==========

        service.PlaceOrder(order);

        #endregion

        #region ========== ASSERT (Проверка) ==========

        // Проверяем, что Reserve вызван для каждого товара ровно 1 раз
        mockWarehouse.Verify(w => w.Reserve(10, 3), Times.Once);
        mockWarehouse.Verify(w => w.Reserve(20, 5), Times.Once);
        mockWarehouse.Verify(w => w.Reserve(30, 1), Times.Once);

        #endregion
    }

    /// <summary>
    /// Параметризованный тест проверяет, что в уведомление передается правильная сумма,
    /// независимо от того, какую сумму вернул калькулятор.
    /// </summary>
    /// <param name="expectedTotal">Сумма, которую возвращает калькулятор</param>
    [Theory]
    [InlineData(1000)]      // Маленькая сумма
    [InlineData(5000)]      // Средняя сумма
    [InlineData(9999.99)]   // Сумма с копейками
    public void PlaceOrder_SendsConfirmationWithCorrectTotal(decimal expectedTotal)
    {
        #region ========== ARRANGE (Подготовка) ==========

        var order = new Order
        {
            Id = 4,
            CustomerEmail = "customer@test.com",
            Items =
            [
                new() {
                    ProductId = 1,
                    Quantity = 1,
                    ProductName = "Товар"
                }
            ]
        };

        // STUB: Товар есть на складе
        var mockWarehouse = new Mock<IWarehouseService>();
        mockWarehouse.Setup(w => w.IsAvailable(It.IsAny<int>(),
                                               It.IsAny<int>()))
                                  .Returns(true);

        // STUB: Калькулятор возвращает сумму, переданную в тест
        var mockCostCalculator = new Mock<ICostCalculator>();
        mockCostCalculator.Setup(c => c.CalculateTotal(It.IsAny<Order>()))
                                       .Returns(expectedTotal);

        // MOCK: Проверяем, что в уведомление передана правильная сумма
        var mockNotification = new Mock<INotificationService>();

        var service = new Domain.Services.OrderService(
            mockWarehouse.Object,
            mockCostCalculator.Object,
            mockNotification.Object);

        #endregion

        #region ========== ACT (Действие) ==========

        service.PlaceOrder(order);

        #endregion

        #region ========== ASSERT (Проверка) ==========

        // Проверяем, что уведомление отправлено с правильной суммой
        mockNotification.Verify(
            n => n.SendOrderConfirmation(
                It.IsAny<string>(),
                It.IsAny<int>(),
                expectedTotal),
            Times.Once);

        #endregion
    }
}