namespace OrderService.Domain.Interfaces
{
    /// <summary>
    /// Предоставляет методы для отправки уведомлений клиентам о статусе их заказов.
    /// </summary>
    /// <remarks>
    /// Реализация интерфейса может использовать различные каналы доставки:
    /// <list type="bullet">
    /// <item><description>Электронная почта (SMTP)</description></item>
    /// <item><description>СМС-уведомления</description></item>
    /// <item><description>Push-уведомления в мобильном приложении</description></item>
    /// <item><description>Telegram-бот</description></item>
    /// </list>
    /// </remarks>
    public interface INotificationService
    {
        /// <summary>
        /// Отправляет клиенту письмо с подтверждением успешного размещения заказа.
        /// </summary>
        /// <param name="customerEmail">Адрес электронной почты клиента.</param>
        /// <param name="orderId">Уникальный идентификатор заказа.</param>
        /// <param name="total">Итоговая стоимость заказа.</param>
        /// <remarks>
        /// Письмо должно содержать информацию о номере заказа и его итоговой стоимости.
        /// </remarks>
        void SendOrderConfirmation(string customerEmail,
                                   int orderId,
                                   decimal total);

        /// <summary>
        /// Отправляет клиенту письмо с уведомлением об ошибке при размещении заказа.
        /// </summary>
        /// <param name="customerEmail">Адрес электронной почты клиента.</param>
        /// <param name="reason">Причина неудачного размещения заказа 
        /// (например, отсутствие товара на складе).</param>
        /// <remarks>
        /// Письмо должно содержать понятное описание причины ошибки
        /// для информирования клиента.
        /// </remarks>
        void SendOrderFailure(string customerEmail,
                              string reason);
    }
}