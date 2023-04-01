const app = getApp();

Page({
    id: "home_index",
    data: {
        topImageHeight: 0,
        homeData: {},
        topImageList: {},
        showFollowPopup: false,
        showHomeNotePopup: false,
    },
    onShow() {
        app.globalData.homePageId = "home_index";

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

                 // 这个 topImageList 单独从 homeData里面拿出来，这样就可以不用每次都更新这个值，造成页面晃动
                if(app.globalData.topImageShowed !== true) {
                    that.setData({
                        topImageList: res.data.homeData.topImageList,
                    });

                    app.globalData.topImageShowed = true;
                }

                // 关注公众号提示
                if (res.data.homeData.hasFollowed !== true) {
                    that.setData({
                        showFollowPopup: true,
                    });
                } else {
                    that.setData({
                        showFollowPopup: false,
                    });
                }
                
                // 首页一键复制地址提示
                if(res.data.homeData.hasFollowed === true && app.globalData.homeNoteShowed === false) {
                    that.setData({
                        showHomeNotePopup: true,
                    });

                    app.globalData.homeNoteShowed = true;
                } else {
                    that.setData({
                        showHomeNotePopup: false,
                    });
                }

                // 存储的是登录时间毫秒值
                wx.setStorageSync('client-login-time', new Date().getTime());
            }
        );
    },

    onTopImageClick(e) {
        const linkUrl = e.currentTarget.dataset.item.linkUrl;
        if (linkUrl && linkUrl !== "") {
            wx.navigateTo({
                url: "/pages/home/home_web_view?url=" + linkUrl,
            });
        }
    },

    onTopImageLoad(e) {
        const that = this;
        let query = wx.createSelectorQuery();
        query.select('.top-image-item').boundingClientRect(r => {
            if (r && r.height) {
                let height = r.height;
                that.setData({
                    topImageHeight: height - 5,
                });
            }
        }).exec();
    },

    onOrderSimpleStepClick(e) {
        app.post("/client/order/simple/step/detail", {}, function (res) {
            wx.navigateTo({
                url: "/pages/home/home_web_view?url=" + res.data.url + "&title=" + res.data.title,
            });
        });
    },

    onTabbarChange(e) {
        if (e.detail === 0) {
            wx.reLaunch({
                url: "./home_index",
            });
        }

        if (e.detail === 1) {
            wx.reLaunch({
                url: "./home_package",
            });
        }

        if (e.detail === 2) {
            wx.reLaunch({
                url: "./home_personal",
            });
        }
    },

    onFollowPopupClose(e) {
        this.setData({
            showFollowPopup: false,
        });
    },

    onHomeNotePopupClose(e) {
        this.setData({
            showHomeNotePopup: false,
        });
    },
})