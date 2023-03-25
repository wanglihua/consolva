const app = getApp();

Page({
    data: {
        "loginBtnText": "自动登录中",
        serviceAgreementUrl: "",
        privacyStatementUrl: "",
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
                                loginBtnText: "立即登录",
                            });
                        });

                    }
                );
            }
        });
    },

    onLoginClick(e) {
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
    }
})