const app = getApp();

Page({
    id: "index_personal",
    data: {
        homeData: {},
        showFollowPopup: false,
    },
    onShow() {
        const nowMilliSeconds = new Date().getTime();
        const loginMilliSeconds = wx.getStorageSync("client-login-time");
        const that = this;
        // 最多5分钟尝试 wx.login() 一次
        if (!loginMilliSeconds || nowMilliSeconds - loginMilliSeconds > 5 * 60 * 1000) {
            wx.login({
                success: res => {
                    that.getClientHomeData(res.code);
                }
            });
        } else {
            that.getClientHomeData("");
        }
    },

    getClientHomeData(code) {
        const that = this;
        app.postWithoutLoading(
            "/client/home/data", {
                code: code
            }, function (res) {
                that.setData({
                    homeData: res.data.homeData,
                });

                if (res.data.homeData.hasFollowed === false) {
                    that.setData({
                        showFollowPopup: true,
                    });
                }

                // 存储的是登录时间毫秒值
                wx.setStorageSync('client-login-time', new Date().getTime());
            }
        );
    },

    onTabbarChange(e) {
        if (e.detail === 0) {
            wx.redirectTo({
                url: "./home_index",
            });
        }

        if (e.detail === 1) {
            wx.redirectTo({
                url: "./home_package",
            });
        }

        if (e.detail === 2) {
            wx.redirectTo({
                url: "./home_personal",
            });
        }
    }
})