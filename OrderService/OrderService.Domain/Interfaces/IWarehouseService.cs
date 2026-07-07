namespace OrderService.Domain.Interfaces
{
    /// <summary>
    /// Предоставляет методы для проверки наличия товаров на складе и их резервирования.
    /// </summary>
    /// <remarks>
    /// Реализация интерфейса может взаимодействовать с:
    /// <list type="bullet">
    /// <item><description>Реляционной базой данных (SQL Server, PostgreSQL)</description></item>
    /// <item><description>Внешним API склада</description></item>
    /// <item><description>In-memory хранилищем для тестирования</description></item>
    /// </list>
    /// </remarks>
    public interface IWarehouseService
    {
        /// <summary>
        /// Проверяет, доступно ли на складе указанное количество товара.
        /// </summary>
        /// <param name="productId">Уникальный идентификатор товара.</param>
        /// <param name="quantity">Запрашиваемое количество единиц товара.</param>
        /// <returns>
        /// <c>true</c> — если товар доступен в нужном количестве;
        /// <c>false</c> — если товара недостаточно или он отсутствует.
        /// </returns>
        bool IsAvailable(int productId, int quantity);

        /// <summary>
        /// Резервирует указанное количество товара на складе.
        /// </summary>
        /// <param name="productId">Уникальный идентификатор товара.</param>
        /// <param name="quantity">Количество единиц товара для резервирования.</param>
        /// <remarks>
        /// Метод должен вызываться только после успешной проверки
        /// через <see cref="IsAvailable"/>. При резервировании количество
        /// товара на складе уменьшается на указанное значение, чтобы
        /// другие заказы не могли использовать этот товар.
        /// </remarks>
        void Reserve(int productId, int quantity);
    }
}