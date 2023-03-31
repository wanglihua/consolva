using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using Newtonsoft.Json;
using shunda.Domain.Wechat;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户
/// </summary>
public class CustomerInfo : Entity {
    public static readonly string DefaultPassword = "111111"; // 6 个 1

    /// <summary>
    /// 入库校验，非空
    /// </summary>
    [Required(ErrorMessage = "入库校验不能为空！")]
    [MaxLength(10, ErrorMessage = "入库校验长度不能大于10！")]
    [JsonProperty("checkinCode")]
    public virtual string CheckinCode { get; set; } = string.Empty;

    /// <summary>
    /// 客户国家，非空
    /// </summary>
    [Required(ErrorMessage = "客户国家不能为空！")]
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 微信小程序OpenId，非空
    /// </summary>
    [Required(ErrorMessage = "微信小程序OpenId不能为空！")]
    [MaxLength(100, ErrorMessage = "微信小程序OpenId长度不能大于100！")]
    [JsonProperty("wechatAppOpenId")]
    public virtual string WechatAppOpenId { get; set; } = string.Empty;

    /// <summary>
    /// 微信公众号OpenId，非空
    /// </summary>
    [Required(ErrorMessage = "微信公众号OpenId不能为空！")]
    [MaxLength(100, ErrorMessage = "微信公众号OpenId长度不能大于100！")]
    [JsonProperty("wechatOfficialAccountOpenId")]
    public virtual string WechatOfficialAccountOpenId { get; set; } = string.Empty;

    /// <summary>
    /// 微信公众号关注状态，非空
    /// </summary>
    [JsonProperty("wechatOfficialAccountFollowStatus")]
    public virtual WechatOfficialAccountFollowStatus WechatOfficialAccountFollowStatus { get; set; } = WechatOfficialAccountFollowStatus.None;

    /// <summary>
    /// 微信公众号关注时间，可空
    /// </summary>
    [JsonProperty("wechatOfficialAccountFollowTime")]
    public virtual DateTime? WechatOfficialAccountFollowTime { get; set; }

    /// <summary>
    /// 微信公众号取消关注时间，可空
    /// </summary>
    [JsonProperty("wechatOfficialAccountUnfollowTime")]
    public virtual DateTime? WechatOfficialAccountUnfollowTime { get; set; }

    /// <summary>
    /// 微信UnionId，非空
    /// </summary>
    [Required(ErrorMessage = "微信UnionId不能为空！")]
    [MaxLength(100, ErrorMessage = "微信UnionId长度不能大于100！")]
    [JsonProperty("wechatUnionId")]
    public virtual string WechatUnionId { get; set; } = string.Empty;

    /// <summary>
    /// 手机号码，非空
    /// </summary>
    [Required(ErrorMessage = "手机号码不能为空！")]
    [MaxLength(20, ErrorMessage = "手机号码长度不能大于20！")]
    [JsonProperty("mobile")]
    public virtual string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// 手机验证，非空
    /// </summary>
    [Required(ErrorMessage = "手机验证不能为空！")]
    [JsonProperty("mobileValidated")]
    public virtual bool MobileValidated { get; set; } = false;

    /// <summary>
    /// 密码salt，非空
    /// </summary>
    [Required(ErrorMessage = "密码salt不能为空！")]
    [MaxLength(20, ErrorMessage = "密码salt长度不能大于20！")]
    [JsonProperty("salt")]
    public virtual string Salt { get; set; } = string.Empty;

    /// <summary>
    /// 密码，非空
    /// </summary>
    [Required(ErrorMessage = "密码不能为空！")]
    [MaxLength(32, ErrorMessage = "密码长度不能大于32！")]
    [JsonProperty("password")]
    public virtual string Password { get; set; } = string.Empty;

    /// <summary>
    /// 支付密码，非空
    /// </summary>
    [Required(ErrorMessage = "支付密码不能为空！")]
    [MaxLength(32, ErrorMessage = "支付密码长度不能大于32！")]
    [JsonProperty("payPassword")]
    public virtual string PayPassword { get; set; } = string.Empty;

    /// <summary>
    /// 注册时间，非空
    /// </summary>
    [Required(ErrorMessage = "注册时间不能为空！")]
    [JsonProperty("regTime")]
    public virtual DateTime RegTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 最后登录时间，非空
    /// </summary>
    [Required(ErrorMessage = "最后登录时间不能为空！")]
    [JsonProperty("lastLogin")]
    public virtual DateTime LastLogin { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 最后登录ip，非空
    /// </summary>
    [Required(ErrorMessage = "最后登录ip不能为空！")]
    [MaxLength(200, ErrorMessage = "最后登录ip长度不能大于200！")]
    [JsonProperty("lastIp")]
    public virtual string LastIp { get; set; } = string.Empty;

    /// <summary>
    /// 用户金额，非空
    /// </summary>
    [Required(ErrorMessage = "用户金额不能为空！")]
    [JsonProperty("account")]
    public virtual decimal Account { get; set; } = 0m;

    /// <summary>
    /// 总积分，非空
    /// </summary>
    [Required(ErrorMessage = "总积分不能为空！")]
    [JsonProperty("totalScore")]
    public virtual decimal TotalScore { get; set; } = 0m;

    /// <summary>
    /// 有效积分，非空
    /// </summary>
    [Required(ErrorMessage = "有效积分不能为空！")]
    [JsonProperty("validScore")]
    public virtual decimal ValidScore { get; set; } = 0m;

    /// <summary>
    /// 累计消费，非空
    /// </summary>
    [Required(ErrorMessage = "累计消费不能为空！")]
    [JsonProperty("totalConsume")]
    public virtual decimal TotalConsume { get; set; } = 0m;

    /// <summary>
    /// 客户级别，非空
    /// </summary>
    [Required(ErrorMessage = "客户级别不能为空！")]
    [JsonProperty("level")]
    [ModelBinder(typeof(CustomerLevelModelBinder))]
    public virtual CustomerLevel Level { get; set; } = CustomerLevel.None;

    /// <summary>
    /// VIP等级，非空
    /// </summary>
    [Required(ErrorMessage = "VIP等级不能为空！")]
    [JsonProperty("vipRank")]
    [ModelBinder(typeof(CustomerVipRankModelBinder))]
    public virtual CustomerVipRank VipRank { get; set; } = CustomerVipRank.None;

    /// <summary>
    /// 客户性质，非空
    /// </summary>
    [Required(ErrorMessage = "客户性质不能为空！")]
    [MaxLength(30, ErrorMessage = "客户性质长度不能大于30！")]
    [JsonProperty("customerCharacter")]
    public virtual string CustomerCharacter { get; set; } = string.Empty;

    /// <summary>
    /// 创建方式，非空
    /// </summary>
    [Required(ErrorMessage = "创建方式不能为空！")]
    [JsonProperty("createMethod")]
    [ModelBinder(typeof(CustomerCreateMethodModelBinder))]
    public virtual CustomerCreateMethod CreateMethod { get; set; } = CustomerCreateMethod.None;

    /// <summary>
    /// 是否已同意 服务协议和隐私条款
    /// </summary>
    public virtual bool Agreed { get; set; }

    /// <summary>
    /// 澳拼客户ID
    /// </summary>
    public virtual int AoPinId { get; set; }

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    public virtual CustomerStatus Status { get; set; } = CustomerStatus.None;

    #region cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}
