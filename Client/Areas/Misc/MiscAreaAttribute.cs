using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Misc;

public class MiscArea {
    public static string Name => "Misc";
}

// 这个里面包含 各种杂项，如果某个杂项后来有更好的归属，就搬过去

public class MiscAreaAttribute : AreaAttribute {
    public MiscAreaAttribute() : base(MiscArea.Name) {
    }
}