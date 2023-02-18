// components/header.js
const app = getApp();
const util = app.require('utils/util.js');

Component({
    options: {
        styleIsolation: 'isolated'
    },

    properties: {},

    data: {
        customerCode: "",
    },

    pageLifetimes: {
        show: function () {
            const nowMilliSeconds = new Date().getTime();
            const loginMilliSeconds = wx.getStorageSync("client-login-time");
            const that = this;
            // 最多5分钟尝试 wx.login() 一次
            if (!loginMilliSeconds || nowMilliSeconds - loginMilliSeconds > 5 * 60 * 1000) {
                wx.login({
                    success: res => {
                        that.getClientHeaderData(res.code);
                    }
                });
            } else {
                that.getClientHeaderData("");
            }
        }
    },

    methods: {
        onClickLeft() {
            // wx.navigateTo({
            //     url: "/pages/home/home_index",
            // });

            // app.goback("index");

            wx.redirectTo({
                url: "/pages/home/home_index",
            });

        },

        onClickRight() {
        },

        getClientHeaderData(code) {
            const that = this;
            app.postWithoutLoading(
                "/client/header/data", {
                    code: code
                }, function (res) {
                    that.setData({
                        customerCode: res.data.customerCode,
                    });

                    // 存储的是登录时间毫秒值
                    wx.setStorageSync('client-login-time', new Date().getTime());
                }
            );
        },
    },
})
 