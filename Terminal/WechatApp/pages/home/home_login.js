const app = getApp();

Page({
    data: {
        serviceAgreementUrl: "",
        privacyStatementUrl: "",
        agreed: false,
        showLoginBtn: false,
    },

    onLoad(options) {
        // const returnUrl = options.returnUrl;

        const that = this;
        wx.login({
            success: res => {
                app.postWithoutLoading(
                    "/client/wechat/app/login", {
                        code: res.code
                    }, function (res) {
                        if (res.data.agreed === true) {
                            wx.redirectTo({
                                url: "./home_index",
                            });
                            return;
                        }

                        app.post("/client/wechat/app/show/login", {}, function (res) {
                            that.setData({
                                serviceAgreementUrl: res.data.serviceAgreementUrl,
                                privacyStatementUrl: res.data.privacyStatementUrl,
                                showLoginBtn: true,
                            });
                        });

                    }
                );
            }
        });
    },

    onLoginClick(e) {
        if (this.data.agreed !== true) {
            wx.showModal({
                title: '错误',
                content: "请您同意 用户服务协议 和 隐私协议 再继续！",
                showCancel: false
            });
            return;
        }

        wx.login({
            success: res => {
                app.post(
                    "/client/wechat/app/login", {
                        code: res.code,
                        agreed: true,
                    }, function (res) {
                        if (res.data.agreed === true) {
                            wx.redirectTo({
                                url: "./home_index",
                            });
                        }
                    }
                );
            }
        });
    },

    onAgreeCheckChange(e) {
        this.setData({
            agreed: e.detail,
        });
    },

    onServiceAgreementClick(e) {
        console.log(e);

        const serviceAgreementUrl = this.data.serviceAgreementUrl;
        wx.navigateTo({
            url: "/pages/home/home_web_view?url=" + serviceAgreementUrl + "&title=用户服务协议",
        });
    },

    onPrivacyStatementClick(e) {
        console.log(e);

        const privacyStatementUrl = this.data.privacyStatementUrl;
        wx.navigateTo({
            url: "/pages/home/home_web_view?url=" + privacyStatementUrl + "&title=隐私协议",
        });
    }
})
