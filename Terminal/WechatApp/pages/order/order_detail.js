const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        orderDetail: {},
        orderShow: {
            show: false,
            text: "显示",
        },
        firstModeShow: {
            show: false,
            text: "显示",
        },
        receiveAddressShow: {
            show: false,
            text: "显示",
        },
        expressListShow: {
            show: false,
            text: "显示",
        },
        orderPackListShow: {
            show: false,
            text: "显示",
        },
        feeShow: {
            show: false,
            text: "显示",
        },
        photoShow: {
            show: false,
            text: "显示",
        }
    },

    onLoad(options) {
        const orderId = options.id;
        this.loadOrderDetail(orderId);
    },

    loadOrderDetail(orderId) {
        const that = this;
        app.post("/client/order/detail", {
            orderId: orderId
        }, function (res) {
            const orderDetail = res.data.orderDetail;
            that.setData({
                orderDetail: orderDetail,
            });
        });
    },

    onCopyClick(e) {
        const copyText = e.currentTarget.dataset.item;
        wx.setClipboardData({
            data: copyText,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    },

    onOrderHeaderClick(e) {
        const orderShow = this.data.orderShow;
        if (orderShow.show) {
            orderShow.show = false;
            orderShow.text = "显示";
        } else {
            orderShow.show = true;
            orderShow.text = "隐藏";
        }

        this.setData({
            orderShow: orderShow,
        });
    },

    onFirstModeHeaderClick(e) {
        const firstModeShow = this.data.firstModeShow;
        if (firstModeShow.show) {
            firstModeShow.show = false;
            firstModeShow.text = "显示";
        } else {
            firstModeShow.show = true;
            firstModeShow.text = "隐藏";
        }

        this.setData({
            firstModeShow: firstModeShow,
        });
    },

    onReceiveAddressHeaderClick(e) {
        const receiveAddressShow = this.data.receiveAddressShow;
        if (receiveAddressShow.show) {
            receiveAddressShow.show = false;
            receiveAddressShow.text = "显示";
        } else {
            receiveAddressShow.show = true;
            receiveAddressShow.text = "隐藏";
        }

        this.setData({
            receiveAddressShow: receiveAddressShow,
        });
    },

    onExpressListHeaderClick(e) {
        const expressListShow = this.data.expressListShow;
        if (expressListShow.show) {
            expressListShow.show = false;
            expressListShow.text = "显示";
        } else {
            expressListShow.show = true;
            expressListShow.text = "隐藏";
        }

        this.setData({
            expressListShow: expressListShow,
        });
    },

    onOrderPackListHeaderClick(e) {
        const orderPackListShow = this.data.orderPackListShow;
        if (orderPackListShow.show) {
            orderPackListShow.show = false;
            orderPackListShow.text = "显示";
        } else {
            orderPackListShow.show = true;
            orderPackListShow.text = "隐藏";
        }

        this.setData({
            orderPackListShow: orderPackListShow,
        });
    },

    onFeeHeaderClick(e) {
        const feeShow = this.data.feeShow;
        if (feeShow.show) {
            feeShow.show = false;
            feeShow.text = "显示";
        } else {
            feeShow.show = true;
            feeShow.text = "隐藏";
        }

        this.setData({
            feeShow: feeShow,
        });
    },

    onPhotoHeaderClick(e) {
        const photoShow = this.data.photoShow;
        if (photoShow.show) {
            photoShow.show = false;
            photoShow.text = "显示";
        } else {
            photoShow.show = true;
            photoShow.text = "隐藏";
        }

        this.setData({
            photoShow: photoShow,
        });
    },

    onViewExpressDetailClick(e) {
        const expressId = e.currentTarget.dataset.item.id;
        wx.navigateTo({
            url: '/pages/express/express_checkin/express_checkin_detail?id=' + expressId,
        });
    },

    onSmallPhotoClick(e) {
        console.log(e);
        const photo = e.currentTarget.dataset.item;
        this.setData({
            showBigPhoto: true,
            bigPhoto: photo,
        });
    },

    onBigPhotoClose(e) {
        this.setData({
            showBigPhoto: false,
        });
    },
})