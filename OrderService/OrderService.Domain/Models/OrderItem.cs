namespace OrderService.Domain.Models
{
    /// <summary>
    /// Представляет позицию в заказе — отдельный товар с указанием количества.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Уникальный идентификатор товара.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Количество единиц товара, заказанных клиентом.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Название товара. Используется для формирования сообщений об ошибках,
        /// когда товара нет в наличии.
        /// </summary>
        public string ProductName { get; set; } = null!;
    }
}