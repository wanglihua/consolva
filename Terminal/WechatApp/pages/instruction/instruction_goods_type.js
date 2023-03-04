// pages/instruction/instruction_goods_type.js
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
        app.post("/client/goods/type/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    },
})