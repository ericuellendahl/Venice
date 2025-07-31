using Venice.Application.DTOs;

namespace Venice.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<OrderResponseDto?> GetOrderByIdAsync(Guid id);
}
