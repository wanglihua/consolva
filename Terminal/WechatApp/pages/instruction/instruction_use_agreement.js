// pages/instruction/instruction_use_agreement.js
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
        app.post("/client/use/agreement/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    }
})