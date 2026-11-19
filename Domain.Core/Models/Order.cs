using Domain.Core.Common;

namespace Domain.Core.Models;

public class Order : EntityBase
{
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<OrderItem> Items { get; set; } = new();
    public decimal DiscountPercent { get; set; }
    public bool IsCompleted { get; set; }

    public void AddItem(Product product, int quantity)
    {
        var item = Items.FirstOrDefault(x => x.ProductId == product.Id);

        if (item is null)
        {
            Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            });
        }
        else
        {
            item.Quantity += quantity;
        }
    }

    public decimal GetSubtotal() => Items.Sum(item => item.GetTotal());

    public override void Validate()
    {
        if (CustomerId == Guid.Empty)
            throw new ArgumentException("Замовлення повинно мати клієнта.");
    }
}
