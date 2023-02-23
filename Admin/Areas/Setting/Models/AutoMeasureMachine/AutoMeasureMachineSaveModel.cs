using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Setting.Models.AutoMeasureMachine;

public class AutoMeasureMachineSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int AutoMeasureMachineId { get; set; }

    /// <summary>
    /// 名称，非空
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(100, ErrorMessage = "名称长度不能大于100！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述，可空
    /// </summary>
    [MaxLength(300, ErrorMessage = "描述长度不能大于300！")]
    public string? Description { get; set; }

    /// <summary>
    /// 仓库，非空
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// app key，非空
    /// </summary>
    [Required(ErrorMessage = "app key不能为空！")]
    [MaxLength(50, ErrorMessage = "app key长度不能大于50！")]
    public string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// secret，非空
    /// </summary>
    [Required(ErrorMessage = "secret不能为空！")]
    [MaxLength(50, ErrorMessage = "secret长度不能大于50！")]
    public string Secret { get; set; } = string.Empty;
}