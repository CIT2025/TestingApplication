namespace OrderService.Domain.Models
{
    /// <summary>
    /// Представляет результат операции размещения заказа.
    /// Содержит информацию об успехе или неудаче, а также сопутствующие данные.
    /// </summary>
    public class OrderResult
    {
        /// <summary>
        /// Указывает, был ли заказ успешно размещен.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Уникальный идентификатор заказа (заполняется только при успешном размещении).
        /// </summary>
        public int OrderId { get; }

        /// <summary>
        /// Итоговая стоимость заказа (заполняется только при успешном размещении).
        /// </summary>
        public decimal TotalAmount { get; }

        /// <summary>
        /// Сообщение об ошибке (заполняется только при неудачном размещении).
        /// </summary>
        public string ErrorMessage { get; }

        private OrderResult(bool isSuccess, 
                            int orderId, 
                            decimal totalAmount, 
                            string errorMessage)
        {
            IsSuccess = isSuccess;
            OrderId = orderId;
            TotalAmount = totalAmount;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Создает результат успешного размещения заказа.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <param name="totalAmount">Итоговая стоимость заказа.</param>
        /// <returns>Объект OrderResult с признаком успеха.</returns>
        public static OrderResult Success(int orderId, decimal totalAmount)
            => new OrderResult(true, orderId, totalAmount, null!);

        /// <summary>
        /// Создает результат неудачного размещения заказа.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке (например, о недостатке товара).</param>
        /// <returns>Объект OrderResult с признаком неудачи.</returns>
        public static OrderResult Failure(string errorMessage)
            => new OrderResult(false, 0, 0, errorMessage);
    }
}