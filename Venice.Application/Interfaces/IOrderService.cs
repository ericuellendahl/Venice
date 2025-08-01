using Venice.Application.DTOs;

namespace Venice.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken);
    Task<OrderResponseDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
}
