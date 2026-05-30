using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Products.DeleteProduct;

public class DeleteProductCommand
{
    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
