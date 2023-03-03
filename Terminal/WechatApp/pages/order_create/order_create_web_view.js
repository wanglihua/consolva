// pages/order_create/order_create_web_view.js
Page({
    data: {
        url: ""
    },

    onLoad(options) {
        const title = options.title;
        const url = options.url;

        this.setData({
            url: url,
        });

        wx.setNavigationBarTitle({
            title: title,
        });
    }
})