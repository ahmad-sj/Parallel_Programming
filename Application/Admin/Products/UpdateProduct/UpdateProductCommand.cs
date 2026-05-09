using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Products.UpdateProduct;

public class UpdateProductCommand
{
    public Guid ProductId { get; set; }
    public int ProductQty { get; set; }

    public UpdateProductCommand(Guid productId, int productQty)
    {
        ProductId = productId;
        ProductQty = productQty;
    }
}
