using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Storage;

public class StorageArea {
    public static string Name => "Storage";
}

public class StorageAreaAttribute : AreaAttribute {
    public StorageAreaAttribute() : base(StorageArea.Name) {
    }
}