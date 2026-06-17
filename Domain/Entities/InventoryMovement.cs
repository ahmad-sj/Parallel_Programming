using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InventoryMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public InventoryMovementType Type { get; set; }
        public int Qty { get; set; }
    }

    public enum InventoryMovementType
    {
        Increase = 1,
        Decrease = 2
    }
}
