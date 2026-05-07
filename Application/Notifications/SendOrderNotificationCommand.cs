using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notifications;

public class SendOrderNotificationCommand
{
    public Guid UserId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Qty { get; set; }
}
