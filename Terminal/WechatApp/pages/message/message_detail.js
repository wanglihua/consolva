Page({
    data: {
        type: "",
        customerCode: "",
        orderCode: "",
        sendTime: "",
        content: "",
        url: "",
        urlTitle: "",
    },

    onLoad(options) {
        const messageId = options.id;

        const that = this;
        app.post("/client/customer/message/detail", {
            messageId: messageId
        }, function (res) {
            const data = res.data;
            that.setData({
                type: data.type,
                customerCode: data.customerCode,
                orderCode: data.orderCode,
                sendTime: data.sendTime,
                content: data.content,
                url: data.url,
                urlTitle: data.urlTitle,
            });
        });
    },

    onOpenPageClick(e) {
        const url = this.data.url;
        wx.navigateTo({
            url: url,
        });
    }
})
