using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLocSpec;

public class StorageLocSpecSaveModel {
    public StorageLocSpecInfo StorageLocSpec { get; set; } = new();
}