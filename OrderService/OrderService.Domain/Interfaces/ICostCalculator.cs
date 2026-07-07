using OrderService.Domain.Models;

namespace OrderService.Domain.Interfaces
{
    /// <summary>
    /// Предоставляет метод для расчета общей стоимости заказа.
    /// </summary>
    /// <remarks>
    /// Реализация интерфейса может учитывать различные факторы:
    /// <list type="bullet">
    /// <item><description>Сумму цен всех товаров с учетом их количества</description></item>
    /// <item><description>Применение скидок и акционных предложений</description></item>
    /// <item><description>Налоговые ставки</description></item>
    /// <item><description>Стоимость доставки</description></item>
    /// </list>
    /// </remarks>
    public interface ICostCalculator
    {
        /// <summary>
        /// Рассчитывает общую стоимость заказа.
        /// </summary>
        /// <param name="order">Заказ, содержащий список товаров с их количеством.</param>
        /// <returns>Общая стоимость заказа в виде десятичного числа.</returns>
        decimal CalculateTotal(Order order);
    }
}