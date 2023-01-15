// pages/instruction/instruction_insurance_clause.js
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
        app.post("/client/insurance/clause/detail", {}, function (res) {
            that.setData({
                title: res.data.title,
                content: res.data.content,
            });
        });
    }
})