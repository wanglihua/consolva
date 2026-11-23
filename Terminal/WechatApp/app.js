// app.js
App({
    onLaunch(object) {
    },
    onShow(object) {
        const that = this;
        // 登录
        wx.login({
            success: res => {
                that.postWithoutLoading(
                    "/client/wechat/app/login", {
                        code: res.code
                    }, function (res) {
                        // 存储的是登录时间毫秒值
                        wx.setStorageSync('client-login-time', new Date().getTime());
                    }
                );
            }
        });
    },
    onHide() {
    },
    onError(error) {
        wx.showModal({
            title: '错误',
            content: "系统发生严重错误：" + error,
            showCancel: false
        });
    },
    onPageNotFound(object) {
    },
    globalData: {
        loginCustomer: {},
        runtimeEnv: "", // 当前运行环境，是不是正式的发布版本
        homePageId: "home_index",
        topImageShowed: false,
        homeNoteShowed: false,
    },
    // 可以 “绝对” 路径引入依赖
    require($uri) {
        return require($uri);
    },
    // 基本上所有请求都是 post
    post(relativeUrl, data, successHandler) {
        wx.showLoading({
            title: '',
            mask: true
        });

        const sessionId = wx.getStorageSync("session-id");
        const clientId = this.getClientId();
        const baseUrl = this.getBaseUrl();
        wx.request({
            url: baseUrl + relativeUrl,
            method: "POST",
            header: {
                "session-id": sessionId,
                "client-id": clientId
            },
            data: data,
            success: function (res) {
                wx.hideLoading({
                    fail() {}
                });

                const sessionId = res.header["session-id"];
                if (sessionId) {
                    wx.setStorageSync('session-id', sessionId);
                }

                if (res.statusCode !== 200) {
                    wx.showModal({
                        title: '错误',
                        content: "请求发生错误，status code  " + res.statusCode,
                        showCancel: false
                    });

                    wx.setStorageSync('client-login-time', 0); // 网络失败的时候，可以重新尝试wx.login()

                    return;
                }

                const success = res.data.success;
                const msg = res.data.message;
                if (success === false) {
                    wx.showModal({
                        title: '错误',
                        content: msg,
                        showCancel: false
                    });

                    wx.setStorageSync('client-login-time', 0); // 处理失败的时候，可以重新尝试wx.login()

                    return;
                }

                if (successHandler) {
                    successHandler(res.data);
                }
            },
            fail: function (res) {
                wx.hideLoading({
                    fail() {}
                });

                wx.showModal({
                    title: '错误',
                    content: res.errMsg,
                    showCancel: false
                });
            },
            complete: function (res) {
            },
        });
    },
    postWithoutLoading(relativeUrl, data, successHandler) {
        const sessionId = wx.getStorageSync("session-id");
        const clientId = this.getClientId();
        const baseUrl = this.getBaseUrl();
        wx.request({
            url: baseUrl + relativeUrl,
            method: "POST",
            header: {
                "session-id": sessionId,
                "client-id": clientId
            },
            data: data,
            success: function (res) {
                const sessionId = res.header["session-id"];
                if (sessionId) {
                    wx.setStorageSync('session-id', sessionId);
                }

                if (res.statusCode !== 200) {
                    wx.showModal({
                        title: '错误',
                        content: "请求发生错误，status code  " + res.statusCode,
                        showCancel: false
                    });
                    return;
                }

                const success = res.data.success;
                const msg = res.data.message;
                if (success === false) {
                    wx.showModal({
                        title: '错误',
                        content: msg,
                        showCancel: false
                    });
                    return;
                }

                if (successHandler) {
                    successHandler(res.data);
                }
            },
            fail: function (res) {
                wx.showModal({
                    title: '错误',
                    content: res.errMsg,
                    showCancel: false
                });
            },
            complete: function (res) {
            },
        });
    },
    // 后退到某个指定 id 的 page 
    goback(pageId) {
        if (!pageId) {
            wx.navigateBack();
        }
        let index = -1;
        const pages = getCurrentPages();
        pages.find((page, routeIndex) => {
            if (page.id === pageId) {
                index = routeIndex;
            }
        });

        if (index === -1) {
            wx.navigateBack();
        } else {
            wx.navigateBack({
                delta: pages.length - index - 1,
            });
        }
    },
    // 获取当前运行环境
    getRuntimeEnv() {
        const accountInfo = wx.getAccountInfoSync();
        // develop 开发版, trial 体验版, release 正式版
        return accountInfo.miniProgram.envVersion;
    },
    getBaseUrl() {
        let runtimeEnv = this.globalData.runtimeEnv;
        if (runtimeEnv === "") {
            runtimeEnv = this.getRuntimeEnv();
            this.globalData.runtimeEnv = runtimeEnv;
        }

        if (runtimeEnv === "develop") {
            return "https://test.com";
        }

        return "https://abc.com";
    },
    getClientId() {
        return "wechat-app-20221117";
    }
})