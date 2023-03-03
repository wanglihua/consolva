// pages/instruction/instruction_insurance_clause.js
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
        app.post("/client/insurance/clause/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    },
})