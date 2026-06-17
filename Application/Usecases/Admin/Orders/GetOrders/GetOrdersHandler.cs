using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Orders.GetOrders;

public class GetOrdersHandler
{
    private readonly IRepository _repository;

    public GetOrdersHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderResult>> Handle(GetOrdersCommand command)
    {
        // get all orders from the database with their items
        var orders = await _repository.GetQueryable<Order>()
            .Include(o => o.Items)
            .ToListAsync();

        Helpers.PrintTimestamp($"'{orders.Count}' orders fetched");

        List<OrderResult> result = new List<OrderResult>();
        foreach (var order in orders)
        {
            var orderItems = order.Items.Select(i => new OrderItemResult {
                Id = i.Id, ProductId = i.ProductId, Name = i.Name,
                Price = i.Price, Qty = i.Qty
            }).ToList();

            var orderResult = new OrderResult {
                Id = order.Id, Date = order.Date, PaymentMethod = order.PaymentMethod,
                Paid = order.Paid, UsedrId = order.UsedrId, Items = orderItems
            };
            result.Add(orderResult);
        }
        return result;
    }
}
