// pages/instruction/instruction_storage.js
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