using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Venice.Domain.Entities;

namespace Venice.Infra.Models;

public class OrderItemDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("orderId")]
    public Guid OrderId { get; set; }

    [BsonElement("produto")]
    public string Produto { get; set; } = string.Empty;

    [BsonElement("quantidade")]
    public int Quantidade { get; set; }

    [BsonElement("precoUnitario")]
    public decimal PrecoUnitario { get; set; }

    public static OrderItemDocument FromDomain(Guid orderId, OrderItem item)
    {
        return new OrderItemDocument
        {
            OrderId = orderId,
            Produto = item.Produto,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario
        };
    }

    public OrderItem ToDomain()
    {
        return new OrderItem(Produto, Quantidade, PrecoUnitario);
    }
}
