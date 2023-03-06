const app = getApp();

Page({
    data: {
        id: 0,
        trackingNo: "",
        goodsValue: "",
        description: "",

        trackingNoHasError: false,
        trackingNoErrorMsg: "",
        goodsValueHasError: false,
        goodsValueErrorMsg: "",
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
                goodsValue: res.data.goodsValue,
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

        app.post("/client/express/expect/edit/save", {
            id: this.data.id,
            trackingNo: this.data.trackingNo,
            goodsValue: this.data.goodsValue,
            description: this.data.description,
        }, function (res) {
            if (res.data.claim === true) {
                wx.showModal({
                    title: '成功',
                    content: res.message,
                    showCancel: false,
                    success: function (res) {
                        wx.navigateTo({
                            url: "/pages/express/express_checkin/express_checkin_list",
                        });
                    }
                });
            } else {
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
            }
        });
    },

    onCancelClick() {
        wx.navigateBack();
    }
})