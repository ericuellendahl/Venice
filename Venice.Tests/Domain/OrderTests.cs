using Venice.Domain.Entities;
using Venice.Domain.ValueObjects;

namespace Venice.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Order_ValidData_ShouldCreateOrderWithCorrectTotalValue()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var itens = new List<OrderItem>
        {
            new("Produto A", 2, 15.50m),
            new("Produto B", 3, 10.00m)
        };

            // Act
            var order = new Order(clienteId, itens);

            // Assert
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal(clienteId, order.ClienteId);
            Assert.Equal(OrderStatus.Criado, order.Status);
            Assert.Equal(61.00m, order.ValorTotal); // (2 * 15.50) + (3 * 10.00)
            Assert.True(order.Data <= DateTime.UtcNow);
        }

        [Fact]
        public void OrderItem_InvalidProduct_ShouldThrowArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new OrderItem("", 1, 10.00m));
            Assert.Throws<ArgumentException>(() => new OrderItem("   ", 1, 10.00m));
        }

        [Fact]
        public void OrderItem_InvalidQuantity_ShouldThrowArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new OrderItem("Produto", 0, 10.00m));
            Assert.Throws<ArgumentException>(() => new OrderItem("Produto", -1, 10.00m));
        }

        [Fact]
        public void OrderItem_InvalidPrice_ShouldThrowArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new OrderItem("Produto", 1, 0m));
            Assert.Throws<ArgumentException>(() => new OrderItem("Produto", 1, -10.00m));
        }

        [Fact]
        public void OrderItem_ValidData_ShouldCalculateSubtotalCorrectly()
        {
            // Arrange & Act
            var orderItem = new OrderItem("Produto Teste", 3, 25.50m);

            // Assert
            Assert.Equal("Produto Teste", orderItem.Produto);
            Assert.Equal(3, orderItem.Quantidade);
            Assert.Equal(25.50m, orderItem.PrecoUnitario);
            Assert.Equal(76.50m, orderItem.Subtotal);
        }
    }
}
