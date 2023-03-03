// pages/instruction/instruction_order_process.js
const app = getApp();

Page({
    data: {
        url: "",
    },

    onLoad(options) {
        this.setData({
            url: options.url
        });

        const that = this;
        app.post("/client/order/process/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    },
})