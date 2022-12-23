// pages/express/express_expect/express_expect_add/express_expect_add.js

const app = getApp();

Page({

    /**
     * 页面的初始数据
     */
    data: {
        trackingNo: "",
        description: "",

        trackingNoHasError: false,
        trackingNoErrorMsg: "",
    },

    onSaveClick() {
        if (!this.data.trackingNo) {
            this.setData({
                trackingNoHasError: true,
                trackingNoErrorMsg: "快递单号不能为空！",
            });
            return;
        }

        this.setData({
            trackingNoHasError: false,
            trackingNoErrorMsg: "",
        });

        app.post("/client/express/expect/add/save", {
            trackingNo: this.data.trackingNo,
            description: this.data.description,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            setTimeout(function () {
                wx.navigateBack({
                    complete: function (e) {
                        const pages = getCurrentPages();
                        const prevPage = pages[pages.length - 1];
                        if(prevPage.loadExpressExpectList) {
                            prevPage.loadExpressExpectList();
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