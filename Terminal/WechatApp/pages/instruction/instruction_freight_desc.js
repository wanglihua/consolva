// pages/instruction/instruction_freight_desc.js
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