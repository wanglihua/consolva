using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderClientListService {
    private readonly ISession session;
    public LogisticsOrderClientListService(ISession session) {
        this.session = session;
    }
    
    
    
}

/*


    public List<LogisticsOrderInfo> getCustomerOrderList(Integer customerId, CustomerOrderStatusCate statusCate, String query, Integer preLastId) {

        if (customerId == null || customerId == 0) {
            throw new ValidationException("会员Id不能为空！");
        }

        if (preLastId == null) {
            throw new ValidationException("上一个列表中末尾的数据Id不能为空！");
        }

        LogisticsOrderDbQueryParam queryParam = new LogisticsOrderDbQueryParam();
        queryParam.setCustomerIdQuery(customerId);

        queryParam.setQuery(query);
        // queryParam.setOrderCodeQuery(query);
        // queryParam.setExpressTrackingNoQuery(query);
        // queryParam.setGoodsNameQuery(query);

        if (statusCate != CustomerOrderStatusCate.Pay) {
            queryParam.setIncludeStatusList(customerOrderStatusCateService.getOrderStatusList(statusCate));
        }

        / *
        后来又说显示全部了
        if (statusCate == CustomerOrderStatusCate.Submitted) { // 提交打包的只显示3.0创建的订单
            queryParam.setFeeMethodQuery(LogisticsOrderFeeMethod.Express); // 暂时用计费方式来区分
        }
         * /

        if (statusCate == CustomerOrderStatusCate.Pay) {
            // queryParam.setFeeMethodQuery(LogisticsOrderFeeMethod.Express);

            // queryParam.setHasArrears(true);
            // queryParam.setVersion3Order(true);
            queryParam.setClientPayList(true);
        }

        queryParam.setPreLastId(preLastId);
        queryParam.setSortField(LogisticsOrderInfo.schema.id);
        queryParam.setSortDir(SortDir.DESC);

        queryParam.setSortByArrived(customerOrderStatusCateService.getSortByArrived(statusCate));

        return logisticsOrderMapper.list(queryParam);

        }

*/