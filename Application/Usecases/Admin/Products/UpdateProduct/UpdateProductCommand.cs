using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.Admin.Products.UpdateProduct;
public class UpdateProductCommand: ILockableCommand {
    public int ProductId { get; set; }
    public int ProductQty { get; set; }
    public UpdateProductCommand(int productId, int productQty) {
        ProductId = productId;
        ProductQty = productQty;
    }
    public string LockKey => $"product:{ProductId}";
}


