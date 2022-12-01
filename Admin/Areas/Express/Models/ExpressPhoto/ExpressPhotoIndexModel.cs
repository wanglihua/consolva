using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressPhoto;

public class ExpressPhotoIndexModel
{
    [BindNever]
    public IList<ExpressPhotoType> PhotoTypeList { get; set; } = ExpressPhotoType.GetList();

    [BindNever]
    public bool CanDelete { get; set; }
}