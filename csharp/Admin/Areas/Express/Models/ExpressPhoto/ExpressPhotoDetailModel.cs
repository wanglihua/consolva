using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressPhoto;

public class ExpressPhotoDetailModel : ExpressPhotoSaveModel
{
    [BindNever]
    public IList<ExpressPhotoType> PhotoTypeList { get; set; } = ExpressPhotoType.GetList();

    /// <summary>
    ///     完整的图片URL
    /// </summary>
    [BindNever]
    public string FullImageUrl { get; set; } = string.Empty;
}