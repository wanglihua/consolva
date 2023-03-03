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
    },
})