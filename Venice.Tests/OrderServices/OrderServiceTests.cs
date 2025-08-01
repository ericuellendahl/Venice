using Microsoft.Extensions.Logging;
using Moq;
using Venice.Application.DTOs;
using Venice.Application.Interfaces;
using Venice.Application.Services;
using Venice.Domain.Entities;
using Venice.Domain.Events;
using Venice.Domain.Interfaces;
using Venice.Domain.Repositories;

namespace Venice.Tests.OrderServices
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<ICacheRepository> _cacheServiceMock;
        private readonly OrderService _orderService;
        private readonly Mock<ILogger<OrderService>> _logger;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _cacheServiceMock = new Mock<ICacheRepository>();
            _logger = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _orderItemRepositoryMock.Object,
                _eventPublisherMock.Object,
                _cacheServiceMock.Object,
                _logger.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_ShouldCreateOrderAndPublishEvent()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var createOrderDto = new CreateOrderDto
            {
                ClienteId = clienteId,
                Itens = new List<OrderItemDto>
            {
                new() { Produto = "Produto A", Quantidade = 2, PrecoUnitario = 10.50m },
                new() { Produto = "Produto B", Quantidade = 1, PrecoUnitario = 25.00m }
            }
            };

            var savedOrder = new Order(clienteId, new List<OrderItem>
        {
            new("Produto A", 2, 10.50m),
            new("Produto B", 1, 25.00m)
        });

            _orderRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Order>(), CancellationToken.None))
                .ReturnsAsync(savedOrder);

            _orderItemRepositoryMock
                .Setup(x => x.SaveItemsAsync(It.IsAny<Guid>(), It.IsAny<List<OrderItem>>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _eventPublisherMock
                .Setup(x => x.PublishAsync(It.IsAny<CreateOrderEvent>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.CreateOrderAsync(createOrderDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clienteId, result.ClienteId);
            Assert.Equal(46.00m, result.ValorTotal); // (2 * 10.50) + (1 * 25.00)
            Assert.Equal(2, result.Itens.Count);

            _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Order>(), CancellationToken.None), Times.Once);
            _orderItemRepositoryMock.Verify(x => x.SaveItemsAsync(It.IsAny<Guid>(), It.IsAny<List<OrderItem>>(), CancellationToken.None), Times.Once);
            _eventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<CreateOrderEvent>()), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_OrderExists_ShouldReturnOrderFromCache()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cachedOrder = new OrderResponseDto
            {
                Id = orderId,
                ClienteId = Guid.NewGuid(),
                Data = DateTime.UtcNow,
                Status = "Criado",
                ValorTotal = 100.00m,
                Itens = new List<OrderItemDto>
            {
                new() { Produto = "Produto Teste", Quantidade = 1, PrecoUnitario = 100.00m }
            }
            };

            _cacheServiceMock
                .Setup(x => x.GetAsync<OrderResponseDto>($"order:{orderId}"))
                .ReturnsAsync(cachedOrder);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
            Assert.Equal(100.00m, result.ValorTotal);

            _cacheServiceMock.Verify(x => x.GetAsync<OrderResponseDto>($"order:{orderId}"), Times.Once);
            _orderRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task GetOrderByIdAsync_OrderNotInCache_ShouldReturnOrderFromDatabaseAndCache()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var order = new Order(clienteId, new List<OrderItem>
        {
            new("Produto Teste", 1, 50.00m)
        });

            var orderItems = new List<OrderItem>
        {
            new("Produto Teste", 1, 50.00m)
        };

            _cacheServiceMock
                .Setup(x => x.GetAsync<OrderResponseDto>($"order:{orderId}"))
                .ReturnsAsync((OrderResponseDto?)null);

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(orderId, CancellationToken.None))
                .ReturnsAsync(order);

            _orderItemRepositoryMock
                .Setup(x => x.GetByOrderIdAsync(orderId, CancellationToken.None))
                .ReturnsAsync(orderItems);

            _cacheServiceMock
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<OrderResponseDto>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clienteId, result.ClienteId);

            _cacheServiceMock.Verify(x => x.GetAsync<OrderResponseDto>($"order:{orderId}"), Times.Once);
            _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId, CancellationToken.None), Times.Once);
            _orderItemRepositoryMock.Verify(x => x.GetByOrderIdAsync(orderId, CancellationToken.None), Times.Once);
            _cacheServiceMock.Verify(x => x.SetAsync($"order:{orderId}", It.IsAny<OrderResponseDto>(), TimeSpan.FromMinutes(2)), Times.Once);
        }
    }

}
