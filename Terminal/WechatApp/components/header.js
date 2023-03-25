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
            // app.goback("home_index");
            const homePageId = app.globalData.homePageId;
            if (homePageId === "home_index") {
                wx.reLaunch({
                    url: "/pages/home/home_index",
                });
            } else if (homePageId === "home_package") {
                wx.reLaunch({
                    url: "/pages/home/home_package",
                });
            } else if (homePageId === "home_personal") {
                wx.reLaunch({
                    url: "/pages/home/home_personal",
                });
            } else {
                wx.reLaunch({
                    url: "/pages/home/home_index",
                });
            }
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

                    if (res.data.agreed !== true) {
                        let pages = getCurrentPages();
                        let currentPage = pages[pages.length - 1];
                        let url = "/" + currentPage.route;

                        wx.redirectTo({
                            url: "/pages/home/home_login?returnUrl=" + url,
                        });
                    }
                }
            );
        },
    },
})
 