using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.ReceivePack;

public class ReceivePackArea {
    public static string Name => "ReceivePack";
}

public class ReceivePackAreaAttribute : AreaAttribute {
    public ReceivePackAreaAttribute() : base(ReceivePackArea.Name) {
    }
}