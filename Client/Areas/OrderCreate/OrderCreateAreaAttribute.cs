using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.OrderCreate;

public class OrderCreateArea {
    public static string Name => "OrderCreate";
}

// 这个里面包含 订单创建，运费预估，订单列表 等

public class OrderCreateAreaAttribute : AreaAttribute {
    public OrderCreateAreaAttribute() : base(OrderCreateArea.Name) {
    }
}