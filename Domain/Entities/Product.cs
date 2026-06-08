using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Qty { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeqId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = [];
    }
}
