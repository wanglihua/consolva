using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Order;

// 这个里面包含 订单创建，运费预估，订单列表 等

public class OrderAreaAttribute : AreaAttribute {
    public static readonly string Name = "Order";

    public OrderAreaAttribute() : base(Name) {
    }
}