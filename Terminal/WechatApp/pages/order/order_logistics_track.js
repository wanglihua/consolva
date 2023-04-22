const app = getApp();

Page({
    data: {
        orderCode: "",
        firstModeName: "",
        goodsType: "",
        trackingNo: "",
        carrierName: "",
        eventList: [],
    },

    onLoad(options) {
        const orderFirstModeId = options.orderFirstModeId;

        const that = this;
        app.post('/client/order/logistics/track', {
            orderFirstModeId: orderFirstModeId,
        }, function (res) {
            that.setData({
                orderCode: res.data.orderCode,
                firstModeName: res.data.firstModeName,
                goodsType: res.data.goodsType,
                trackingNo: res.data.trackingNo,
                carrierName: res.data.carrierName,
                eventList: res.data.eventList,
            });
        });
    }
})