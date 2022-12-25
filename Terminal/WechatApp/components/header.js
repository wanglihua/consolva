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
            // wx.showToast({title: '点击返回', icon: 'none'});
            wx.navigateTo({
                url: "/pages/index/index",
            });
            
            // app.goback("index");
        },
        onClickRight() {
            wx.showToast({title: '点击VIP', icon: 'none'});
        },
    },
})
