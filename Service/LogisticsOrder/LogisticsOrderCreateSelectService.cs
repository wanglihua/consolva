using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateSelectService {
    private readonly ISession session;

    public LogisticsOrderCreateSelectService(ISession session) {
        this.session = session;
    }

    public void SaveCountry(int customerId, Country country) {
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                Country = country,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            createSelect.Country = country;
            createSelect.Valid = false;
            session.Update(createSelect);
        }

        session.Flush();
    }

    public void SaveGoodsType(int customerId, LogisticsGoodsType goodsType) {
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                GoodsType = goodsType,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            createSelect.GoodsType = goodsType;
            createSelect.Valid = false;
            session.Update(createSelect);
        }

        session.Flush();
    }

    public void SaveFirstMode(int customerId, int firstModeId) {
        var firstMode = session.Get<LogisticsModeInfo>(firstModeId);
        if (firstMode == null) {
            throw new ValidationException($"指定的物流方式#{firstModeId}不存在！");
        }

        var firstType = firstMode.Type;

        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                FirstType = firstType,
                FirstModeId = firstModeId,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            if (createSelect.Country != firstMode.Country) {
                throw new ValidationException("物流方式对应国家和订单创建记录国家不一致！");
            }

            createSelect.FirstType = firstType;
            createSelect.FirstModeId = firstModeId;
            createSelect.Valid = false;
            session.Update(createSelect);
        }
    }

    public void SaveReceiveAddress(int customerId, int receiveAddressId) {
        CustomerReceiveAddressInfo receiveAddress = session.Get<CustomerReceiveAddressInfo>(receiveAddressId);
        if (receiveAddress == null) {
            throw new ValidationException($"指定的收货地址#{receiveAddressId}不存在！");
        }

        if (receiveAddress.Customer.Id != customerId) {
            throw new ValidationException($"指定的收货地址#{receiveAddressId}不属于当前客户#{customerId}！");
        }

        // 判断是否有非英文字符
        if (StringUtil.AllEnglish(receiveAddress.FirstName) == false) {
            throw new ValidationException("[first name] 有非英文字符，请更正！");
        }

        if (StringUtil.HasDigit(receiveAddress.FirstName) == true) {
            throw new ValidationException("[first name] 有数字字符，请更正！");
        }

        if (StringUtil.AllEnglish(receiveAddress.LastName) == false) {
            throw new ValidationException("[last name] 有非英文字符，请更正！");
        }

        if (StringUtil.HasDigit(receiveAddress.LastName) == true) {
            throw new ValidationException("[last name] 有数字字符，请更正！");
        }

        if (StringUtil.AllEnglish(receiveAddress.Address) == false) {
            throw new ValidationException("[门牌号/路名] 中有非英文字符，请更正！");
        }

        if (StringUtil.AllEnglish(receiveAddress.City) == false) {
            throw new ValidationException("[区/Suburb] 中有非英文字符，请更正！");
        }

        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                AddressReceiveId = receiveAddressId,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            if (createSelect.Country != receiveAddress.Country) {
                throw new ValidationException("收货地址对应国家和订单创建记录国家不一致！");
            }

            createSelect.AddressReceiveId = receiveAddressId;
            createSelect.Valid = false;
            session.Update(createSelect);
        }

        session.Flush();
    }

    public void SaveSendAddress(int customerId, int sendAddressId) {
        CustomerSendAddressInfo sendAddress = session.Get<CustomerSendAddressInfo>(sendAddressId);
        if (sendAddress == null) {
            throw new ValidationException($"指定的发货地址#{sendAddressId}不存在！");
        }

        if (sendAddress.Customer.Id != customerId) {
            throw new ValidationException($"指定的发货地址#{sendAddressId}不属于当前客户#{customerId}！");
        }

        if (StringUtil.AllChinese(sendAddress.Name) == false) {
            throw new ValidationException("发件人姓名中有非中文字符，请更正！");
        }

        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                AddressSendId = sendAddressId,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            createSelect.AddressSendId = sendAddressId;
            createSelect.Valid = false;
            session.Update(createSelect);
        }

        session.Flush();
    }

    public void SaveInsurance(int customerId, int insuranceId) {
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                InsuranceId = insuranceId,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            createSelect.InsuranceId = insuranceId;
            createSelect.Valid = false;
            session.Update(createSelect);
        }

        session.Flush();
    }

    public void SaveGst(int customerId, int gstId) {
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                GstId = gstId,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            createSelect.GstId = gstId;
            createSelect.Valid = false;
            session.Update(createSelect);
        }

        session.Flush();
    }

    public void SaveCoupon(int customerId, int couponId) {
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            createSelect = new LogisticsOrderCreateSelectInfo {
                CustomerId = customerId,
                CouponId = couponId,
                Valid = false,
            };
            session.Save(createSelect);
        } else {
            createSelect.CouponId = couponId;
            createSelect.Valid = false;
            session.Update(createSelect);
        }

        session.Flush();
    }

    /// <summary>
    /// 标记订单创建选择失效
    /// </summary>
    /// <param name="customerId"></param>
    /// <exception cref="ValidationException"></exception>
    public void MakeInvalid(int customerId) {
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            // throw new ValidationException($"指定的订单创建选择记录#{customerId}不存在！"); 记录不存在也就不报错了
            return;
        }

        createSelect.Valid = false;

        session.Update(createSelect);
        session.Flush();
    }

    /// <summary>
    /// 标记订单创建选择有效
    /// </summary>
    /// <param name="customerId"></param>
    public void MakeValid(int customerId) {
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            throw new ValidationException($"指定的订单创建选择记录#{customerId}不存在！"); // 相比 标记失效时，这个时候报个错比较合适
        }

        createSelect.Valid = true;

        session.Update(createSelect);
        session.Flush();
    }
}