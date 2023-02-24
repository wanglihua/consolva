const app = getApp();

Page({

    data: {
        url: ""
    },

    onLoad(options) {
        const url = options.url;
        this.setData({
            url: url,
        });
    }
})