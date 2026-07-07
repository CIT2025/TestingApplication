using OrderService.Domain.Interfaces;  
using OrderService.Domain.Models;      

namespace OrderService.Domain.Services    
{
    /// <summary>
    /// Сервис для обработки заказов. Реализует бизнес-логику размещения заказа:
    /// проверку наличия товаров, резервирование, расчет стоимости и отправку уведомлений.
    /// </summary>
    public class OrderService
    {
        // Приватное поле для работы со складом (проверка и резервирование товаров)
        private readonly IWarehouseService _warehouseService;

        // Приватное поле для расчета стоимости заказа
        private readonly ICostCalculator _costCalculator;

        // Приватное поле для отправки уведомлений клиенту
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Конструктор сервиса. Все зависимости передаются через конструктор (Constructor Injection).
        /// Это делает класс слабосвязанным и тестируемым.
        /// </summary>
        /// <param name="warehouseService">Сервис склада (проверка и резервирование)</param>
        /// <param name="costCalculator">Калькулятор стоимости заказа</param>
        /// <param name="notificationService">Сервис уведомлений</param>
        public OrderService(IWarehouseService warehouseService,
                            ICostCalculator costCalculator,
                            INotificationService notificationService)
        {
            // Сохраняем переданные зависимости в приватные поля
            _warehouseService = warehouseService;
            _costCalculator = costCalculator;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Размещает заказ: проверяет наличие товаров, резервирует их,
        /// рассчитывает стоимость и отправляет уведомление.
        /// </summary>
        /// <param name="order">Объект заказа с данными клиента и списком товаров</param>
        /// <returns>Результат операции (успех с ID заказа и суммой, либо ошибка)</returns>
        public OrderResult PlaceOrder(Order order)
        {
            // Цикл по всем товарам в заказе
            foreach (var item in order.Items)
            {
                // Проверяем, есть ли товар на складе в нужном количестве
                if (!_warehouseService.IsAvailable(item.ProductId, item.Quantity))
                {
                    // Если товара нет — формируем сообщение об ошибке
                    var error = $"Товар '{item.ProductName}' отсутствует на складе в нужном количестве";

                    // Отправляем клиенту уведомление об ошибке
                    _notificationService.SendOrderFailure(order.CustomerEmail, error);

                    // Возвращаем результат с ошибкой (заказ НЕ создан)
                    return OrderResult.Failure(error);
                }
            }

            // Если все товары есть — резервируем каждый товар на складе
            foreach (var item in order.Items)
                _warehouseService.Reserve(item.ProductId, item.Quantity);  // Списываем товар со склада

            // Рассчитываем общую стоимость заказа (через калькулятор)
            var total = _costCalculator.CalculateTotal(order);

            // Отправляем клиенту подтверждение об успешном размещении заказа
            _notificationService.SendOrderConfirmation(order.CustomerEmail, order.Id, total);

            // Возвращаем успешный результат с ID заказа и итоговой суммой
            return OrderResult.Success(order.Id, total);
        }
    }
}