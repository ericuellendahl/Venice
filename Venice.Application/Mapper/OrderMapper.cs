using Venice.Application.DTOs;
using Venice.Domain.Entities;

namespace Venice.Application.Mapper;

public static class OrderMapper
{
    public static OrderResponseDto ToDto(Order order, List<OrderItem> itens)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            ClienteId = order.ClienteId,
            Data = order.Data,
            Status = order.Status.ToString(),
            ValorTotal = order.ValorTotal,
            Itens = itens.Select(i => new OrderItemDto
            {
                Produto = i.Produto,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList()
        };
    }
}
