// components/header.js
const app = getApp();
const util = app.require('utils/util.js');

Component({
    options: {
        styleIsolation: 'isolated'
    },

    properties: {},

    data: {},

    pageLifetimes: {
        show: function () {
        }
    },

    methods: {
        onClickLeft() {
            wx.showToast({title: '点击logo', icon: 'none'});
            // wx.navigateTo({
            //     url: "/pages/home/home_index",
            // });

            // app.goback("index");

            wx.redirectTo({
                url: "/pages/home/home_index",
            });

        },
        onClickRight() {
            wx.showToast({title: '点击VIP', icon: 'none'});
        },
    },
})
