using Microsoft.Extensions.Logging;
using System.Transactions;
using Venice.Application.DTOs;
using Venice.Application.Interfaces;
using Venice.Application.Mapper;
using Venice.Domain.Entities;
using Venice.Domain.Events;
using Venice.Domain.Interfaces;
using Venice.Domain.Repositories;

namespace Venice.Application.Services;

public class OrderService(IOrderRepository _orderRepository,
                          IOrderItemRepository _orderItemRepository,
                          IEventPublisher _eventPublisher,
                          ICacheRepository _cacheRepository,
                          ILogger<OrderService> logger) : IOrderService
{
    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken)
    {

        Guid rollbackGuidMongotryError = Guid.Empty;

        try
        {
            logger.LogInformation("Creating order for client {ClientId} with {ItemCount} items",
                createOrderDto.ClienteId, createOrderDto.Itens.Count);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var order = new Order(createOrderDto.ClienteId, [.. createOrderDto.Itens.Select(i => new OrderItem(i.Produto, i.Quantidade, i.PrecoUnitario))]);

            var savedOrder = await _orderRepository.CreateAsync(order, cancellationToken);

            logger.LogInformation("Order {OrderId} created with total value {TotalValue}",
                savedOrder.Id, savedOrder.ValorTotal);

            await _orderItemRepository.SaveItemsAsync(savedOrder.Id, [.. createOrderDto.Itens.Select(i => new OrderItem(i.Produto, i.Quantidade, i.PrecoUnitario))], cancellationToken);

            rollbackGuidMongotryError = savedOrder.Id;

            logger.LogInformation("Order {OrderId} created successfully", savedOrder.Id);

            var evento = new CreateOrderEvent(
                savedOrder.Id,
                savedOrder.ClienteId,
                savedOrder.ValorTotal,
                savedOrder.Data,
                [.. createOrderDto.Itens.Select(i => new OrderItem(i.Produto, i.Quantidade, i.PrecoUnitario))]);

            await _eventPublisher.PublishAsync(evento);

            logger.LogInformation("Order creation event published for order {OrderId}", savedOrder.Id);

            scope.Complete();

            return OrderMapper.ToDto(savedOrder, [.. createOrderDto.Itens.Select(i => new OrderItem(i.Produto, i.Quantidade, i.PrecoUnitario))]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar pedido para cliente {Error}", rollbackGuidMongotryError);
            await _orderItemRepository.RemoveAsync(rollbackGuidMongotryError, CancellationToken.None);
            throw new ArgumentException("Erro ao salvar no banco de dados, as informações não persistiram no repositório.");
        }
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";
        var cachedOrder = await _cacheRepository.GetAsync<OrderResponseDto>(cacheKey);
        if (cachedOrder is not null)
        {
            logger.LogInformation("Order {OrderId} retrieved from cache", id);

            return cachedOrder;
        }

        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        logger.LogInformation("Retrieving order {OrderId} from database", id);

        if (order is null) return null;

        var itens = await _orderItemRepository.GetByOrderIdAsync(id, cancellationToken);

        var response = OrderMapper.ToDto(order, itens);

        await _cacheRepository.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2));

        logger.LogInformation("Order {OrderId} cached successfully", id);

        return response;
    }
}
