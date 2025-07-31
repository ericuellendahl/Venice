namespace Venice.Domain.Entities;

public sealed class OrderItem
{
    public string Produto { get; private set; }
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    private OrderItem() { }

    public OrderItem(string produto, int quantidade, decimal precoUnitario)
    {
        if (string.IsNullOrWhiteSpace(produto))
            throw new ArgumentException("Produto não pode ser vazio", nameof(produto));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));

        Produto = produto;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }
}
