using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Orders.GetOrderById;

public class GetOrderByIdHandler
{
    private readonly IRepository _repository;

    public GetOrderByIdHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResult> Handle(GetOrderByIdCommand command)
    {
        var order = await _repository.GetAsync<Domain.Entities.Order>(i => i.Id == command.Id);

        if (order == null)
            throw new Exception("Order not found!");

        var orderItems = order.Items.Select(i => new OrderItemResult
        {
            Id = i.Id,
            ProductId = i.ProductId,
            Name = i.Name,
            Price = i.Price,
            Qty = i.Qty
        }).ToList();

        var result = new OrderResult
        {
            Id = order.Id,
            Date = order.Date,
            PaymentMethod = order.PaymentMethod,
            Paid = order.Paid,
            UsedrId = order.UsedrId,
            Items = orderItems
        };
        return result;
    }
}
