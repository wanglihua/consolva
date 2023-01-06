// pages/order_create/order_create_first_mode.js
const app = getApp();

Page({
    data: {
        firstModeList: [],
        selectFirstModeId: 0,
    },

    onLoad(options) {
        this.loadFirstModeList();
    },

    loadFirstModeList() {
        const that = this;
        app.post("/client/order/create/first/mode/list", {}, function (res) {
            that.setData({
                firstModeList: res.data.firstModeList,
                selectFirstModeId: res.data.selectFirstModeId,
            });
        });
    },

    onFirstModeButtonClick(e) {
        const firstMode = e.currentTarget.dataset.item;
        const firstModeId = firstMode.id;

        const that = this;
        app.post("/client/order/create/first/mode/save", {
            firstModeId: firstModeId,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.setData({
                selectFirstModeId: firstModeId,
            });

            that.gotoNext();
        })
    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {
        const selectFirstModeId = this.data.selectFirstModeId;
        if (selectFirstModeId === 0) {
            wx.showModal({
                title: '错误',
                content: "请选择头程物流！",
                showCancel: false
            });
            return;
        }

        this.gotoNext();
    },

    gotoPre() {
        wx.navigateTo({
            url: "./order_create_country",
        });
    },

    gotoNext() {
        wx.navigateTo({
            url: "./order_create_receive_address",
        });
    }
})
