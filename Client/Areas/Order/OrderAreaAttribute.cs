using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Order;

public class OrderArea {
    public static string Name => "Order";
}

// 这个里面包含 订单创建，运费预估，订单列表 等

public class OrderAreaAttribute : AreaAttribute {
    public OrderAreaAttribute() : base(OrderArea.Name) {
    }
}