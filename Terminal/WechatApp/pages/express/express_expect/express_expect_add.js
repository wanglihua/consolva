// pages/express/express_expect/express_expect_add/express_expect_add.js

const app = getApp();

Page({
    data: {
        trackingNo: "",
        goodsValue: "",
        description: "",

        trackingNoHasError: false,
        trackingNoErrorMsg: "",
        goodsValueHasError: false,
        goodsValueErrorMsg: "",
    },

    onSaveClick() {
        if (!this.data.trackingNo) {
            this.setData({
                trackingNoHasError: true,
                trackingNoErrorMsg: "快递单号不能为空！",
            });
            return;
        }

        if (!this.data.goodsValue) {
            this.setData({
                goodsValueHasError: true,
                goodsValueErrorMsg: "快递单货品总值不能为空！",
            });
            return;
        }

        this.setData({
            trackingNoHasError: false,
            trackingNoErrorMsg: "",
            goodsValueHasError: false,
            goodsValueErrorMsg: "",
        });

        app.post("/client/express/expect/add/save", {
            trackingNo: this.data.trackingNo,
            goodsValue: this.data.goodsValue,
            description: this.data.description,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            setTimeout(function () {
                const pages = getCurrentPages();
                wx.navigateBack({
                    complete: function (e) {
                        if (pages.length >= 2) {
                            const prevPage = pages[pages.length - 2];
                            if (prevPage.loadExpressExpectList) {
                                prevPage.loadExpressExpectList();
                            }
                        }
                    }
                });
            }, 1000);
        });
    },

    onCancelClick() {
        wx.navigateBack();
    }
})