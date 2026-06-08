using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeqId { get; set; }
    }
}
