using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Storage;

public class StorageAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Storage";

    public StorageAreaAttribute() : base(Name)
    {
    }
}