// pages/express/express_expect/express_expect_edit/express_expect_edit.js

const app = getApp();

Page({
    data: {
        id: 0,
        trackingNo: "",
        description: "",

        trackingNoHasError: false,
        trackingNoErrorMsg: "",
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/express/expect/detail", {
            id: id
        }, function (res) {
            that.setData({
                id: id,
                trackingNo: res.data.trackingNo,
                description: res.data.description,
            });
        });
    },

    onSaveClick() {
        if (this.data.trackingNo === "") {
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

        app.post("/client/express/expect/edit/save", {
            id: this.data.id,
            trackingNo: this.data.trackingNo,
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