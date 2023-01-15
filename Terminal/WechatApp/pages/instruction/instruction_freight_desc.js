// pages/instruction/instruction_freight_desc.js
const app = getApp();

Page({
    data: {
        title: "",
        content: "",
    },

    onLoad(options) {
        this.loadInstruction();
    },

    loadInstruction() {
        const that = this;
        app.post("/client/freight/desc/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    }
})