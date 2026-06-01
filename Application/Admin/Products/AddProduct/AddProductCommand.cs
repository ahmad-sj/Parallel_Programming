using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Products.AddProduct;

public class AddProductCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = null!;
    public double Price { get; set; }
    public int Qty { get; set; }
}
