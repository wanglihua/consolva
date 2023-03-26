package com.huadu.shunda.data.http.goodsexpress;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class GoodsExpressQueryResponseInfo extends ResponseInfo {

    @SerializedName("expressTrackingNo")
    private String expressTrackingNo;

    @SerializedName("issueGoods")
    private boolean issueGoods;

    @SerializedName("issueGoodsDesc1")
    private String issueGoodsDesc1;

    @SerializedName("issueGoodsDesc2")
    private String issueGoodsDesc2;

    @SerializedName("claimGoods")
    private boolean claimGoods;

    @SerializedName("claimGoodsDesc1")
    private String claimGoodsDesc1;

    @SerializedName("claimGoodsDesc2")
    private String claimGoodsDesc2;

    @SerializedName("orderExpress")
    private boolean orderExpress;

    @SerializedName("orderExpressDesc")
    private String orderExpressDesc;

    public String getExpressTrackingNo() {
        return expressTrackingNo;
    }

    public void setExpressTrackingNo(String expressTrackingNo) {
        this.expressTrackingNo = expressTrackingNo;
    }

    public boolean isIssueGoods() {
        return issueGoods;
    }

    public void setIssueGoods(boolean issueGoods) {
        this.issueGoods = issueGoods;
    }

    public String getIssueGoodsDesc1() {
        return issueGoodsDesc1;
    }

    public void setIssueGoodsDesc1(String issueGoodsDesc1) {
        this.issueGoodsDesc1 = issueGoodsDesc1;
    }

    public String getIssueGoodsDesc2() {
        return issueGoodsDesc2;
    }

    public void setIssueGoodsDesc2(String issueGoodsDesc2) {
        this.issueGoodsDesc2 = issueGoodsDesc2;
    }

    public boolean isClaimGoods() {
        return claimGoods;
    }

    public void setClaimGoods(boolean claimGoods) {
        this.claimGoods = claimGoods;
    }

    public String getClaimGoodsDesc1() {
        return claimGoodsDesc1;
    }

    public void setClaimGoodsDesc1(String claimGoodsDesc1) {
        this.claimGoodsDesc1 = claimGoodsDesc1;
    }

    public String getClaimGoodsDesc2() {
        return claimGoodsDesc2;
    }

    public void setClaimGoodsDesc2(String claimGoodsDesc2) {
        this.claimGoodsDesc2 = claimGoodsDesc2;
    }

    public boolean isOrderExpress() {
        return orderExpress;
    }

    public void setOrderExpress(boolean orderExpress) {
        this.orderExpress = orderExpress;
    }

    public String getOrderExpressDesc() {
        return orderExpressDesc;
    }

    public void setOrderExpressDesc(String orderExpressDesc) {
        this.orderExpressDesc = orderExpressDesc;
    }
}
