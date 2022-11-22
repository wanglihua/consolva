using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.ReceivePack.Models.ShelfUser;

public class ShelfUserSaveModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int ShelfUserId { get; set; }

    /// <summary>
    /// 仓库，非空
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// 货架，非空
    /// </summary>
    [Required(ErrorMessage = "货架不能为空！")]
    public int ShelfId { get; set; }

    /// <summary>
    /// 用户，非空
    /// </summary>
    [Required(ErrorMessage = "用户登录名不能为空！")]
    public string LoginName { get; set; } = string.Empty;

    /// <summary>
    /// 类型，非空
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(ShelfUserTypeModelBinder))]
    public ShelfUserType Type { get; set; } = ShelfUserType.None;
}