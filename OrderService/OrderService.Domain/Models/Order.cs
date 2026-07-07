namespace OrderService.Domain.Models
{
    /// <summary>
    /// Представляет заказ, размещаемый клиентом в системе интернет-магазина.
    /// Содержит информацию о клиенте и перечне заказанных товаров.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Уникальный идентификатор заказа.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Адрес электронной почты клиента, разместившего заказ.
        /// Используется для отправки уведомлений о статусе заказа.
        /// </summary>
        public string CustomerEmail { get; set; } = null!;

        /// <summary>
        /// Список товаров, входящих в заказ.
        /// Каждый элемент содержит идентификатор товара, его название и количество.
        /// </summary>
        public List<OrderItem> Items { get; set; } = [];
    }
}