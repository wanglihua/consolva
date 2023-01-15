// pages/instruction/instruction_storage.js
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
        app.post("/client/storage/instruction/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    }
})