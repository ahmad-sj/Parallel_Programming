// ============================================================
// المتطلب 4: تعديل بسيط مطلوب على OrderItem Entity
// الملف: Domain/Entities/OrderItem.cs
// ============================================================
// أضف حقل UnitPrice حتى يعمل حساب الإيرادات في DailySalesBatchJob

namespace Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Qty { get; set; }

    // ✅ مطلوب للـ Batch Job حتى يحسب الإيرادات
    public double UnitPrice { get; set; }
}