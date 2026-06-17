using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.Admin.Orders;

public class OrderResult
{
    public int Id { get; set; }
    public List<OrderItemResult> Items { get; set; } = null!;
    public DateTime Date { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public bool Paid { get; set; }
    public int UsedrId { get; set; }
}

public class OrderItemResult
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Qty { get; set; }
}
