// pages/instruction/instruction_use_agreement.js
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