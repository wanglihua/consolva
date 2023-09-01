const app = getApp();

Page({

    data: {
        promotionImageDesc: "",
        // 初始化为9个空的字符串
        promotionImageList: [
            "", "", "",
            "", "", "",
            "", "", "",
        ]
    },

    onLoad(options) {
        let that = this;

        app.post("/client/customer/promotion", {
            trackingNoQuery: that.data.trackingNoQuery,
        }, function (res) {
            const promotionImageDesc = res.data.promotionImageDesc;
            const promotionImageList = res.data.promotionImageList;

            that.setData({
                promotionImageDesc: promotionImageDesc,
                promotionImageList: promotionImageList,
            });
        });
    }
})