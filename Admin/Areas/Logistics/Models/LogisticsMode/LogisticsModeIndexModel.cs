using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsMode;

public class LogisticsModeIndexModel {
    /// <summary>
    /// 国家
    /// </summary>
    [BindNever]
    public Country Country { get; set; } = Country.None;

    [BindNever]
    public IList<Country> CountryList { get; set; } = Country.GetNoChinaList();

    [BindNever]
    public IList<LogisticsModeType> LogisticsModeTypeList { get; set; } = new List<LogisticsModeType>();

    [BindNever]
    public IList<LogisticsModeStatus> StatusList { get; set; } = new List<LogisticsModeStatus>();
}