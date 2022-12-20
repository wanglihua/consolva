using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateSelectService {
    private readonly ISession session;

    public LogisticsOrderCreateSelectService(ISession session) {
        this.session = session;
    }

    public void SaveCountry() {
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