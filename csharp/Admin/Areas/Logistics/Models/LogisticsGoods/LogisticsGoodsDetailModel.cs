using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Admin.Areas.Logistics.Models.LogisticsGoods.Web.Areas.Admin.Models.LogisticsGoods;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

public class LogisticsGoodsDetailModel
{
    public LogisticsGoodsInfo LogisticsGoods { get; set; }

    [BindNever]
    public IList<LogisticsGoodsImageInfo> ImageList { get; set; }

    public IList<LogisticsGoodsImageModel> NewImageList { get; set; }

    #region 海关物品字段

    /// <summary>
    /// 录入方式是自动录入码
    /// </summary>
    public bool HsGoodsAutoSelect { get; set; } = false;

    public int HsGoodsId { get; set; }
    
    //[Required(ErrorMessage = "物品类型不能为空！")]
    public int TypeId { get; set; } // 现在基本没用

    //[Required(ErrorMessage = "中国海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "中国海关编码长度不能大于100！")]
    public string CnHsCode { get; set; }

    //[Required(ErrorMessage = "中国海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "中国海关申报名长度不能大于200！")]
    public string CnHsName { get; set; }

    //[Required(ErrorMessage = "澳洲海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "澳洲海关编码长度不能大于100！")]
    public string AuHsCode { get; set; }

    //[Required(ErrorMessage = "澳洲海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "澳洲海关申报名长度不能大于200！")]
    public string AuHsName { get; set; }

    [MaxLength(300, ErrorMessage = "要素1长度不能大于300！")]
    public string Elem01 { get; set; }

    [MaxLength(300, ErrorMessage = "要素2长度不能大于300！")]
    public string Elem02 { get; set; }

    [MaxLength(300, ErrorMessage = "要素3长度不能大于300！")]
    public string Elem03 { get; set; }

    [MaxLength(300, ErrorMessage = "要素4长度不能大于300！")]
    public string Elem04 { get; set; }

    [MaxLength(300, ErrorMessage = "要素5长度不能大于300！")]
    public string Elem05 { get; set; }

    [MaxLength(300, ErrorMessage = "要素6长度不能大于300！")]
    public string Elem06 { get; set; }

    [MaxLength(300, ErrorMessage = "要素7长度不能大于300！")]
    public string Elem07 { get; set; }

    [MaxLength(300, ErrorMessage = "要素8长度不能大于300！")]
    public string Elem08 { get; set; }

    #endregion

    [BindNever]
    public bool IsCreate { get; set; }

    [BindNever]
    public bool IsViewOnly { get; set; }

    [BindNever]
    public int? StatusId { get; set; }

    [BindNever]
    public IList<LogisticsGoodsCate> CateList { get; set; }

    [BindNever]
    public IList<LogisticsGoodsType> TypeList { get; set; }

    [BindNever]
    public IList<LogisticsGoodsStatus> StatusList { get; set; }

    [BindNever]
    public IList<LogisticsGoodsLogInfo> LogList { get; set; }
}