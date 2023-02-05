namespace shunda.Service.LogisticsOrder; 

public class LogisticsOrderClientListParam {
    
}


/*

public class CustomerOrderListParam {

    // region 订单编号， 快递单号， 物品名称 查询

    @Size(max = 100, message = "查询字符串太长！")
    private String query;

    public String getQuery() {
        return query;
    }

    public void setQuery(String query) {
        this.query = query;
    }

    // endregion

    // region preLastOrderId

    @NotNull(message = "前面列表末尾数据Id不能为空！")
    @JsonProperty("preLastOrderId")
    private Integer preLastOrderId;

    public Integer getPreLastOrderId() {
        return preLastOrderId;
    }

    public void setPreLastOrderId(Integer preLastOrderId) {
        this.preLastOrderId = preLastOrderId;
    }

    // endregion

    // region skip

    private Integer skip;

    public Integer getSkip() {
        return skip;
    }

    public void setSkip(Integer skip) {
        this.skip = skip;
    }

    // endregion
}


*/