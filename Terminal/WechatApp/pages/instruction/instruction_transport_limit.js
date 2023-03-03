// pages/instruction/instruction_transport_limit.js
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
        app.post("/client/transport/limit/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    },
})