// pages/order_create/order_create_repack_type.js
const app = getApp();

Page({
    data: {
        repackTypeList: [],
        selectRepackTypeId: 0,
        remark: "",
    },

    onLoad(options) {
        this.loadRepackTypeList();
    },

    loadRepackTypeList() {
        const that = this;
        app.post("/client/order/create/repack/type/list", {}, function (res) {
            that.setData({
                repackTypeList: res.data.repackTypeList,
                selectRepackTypeId: res.data.selectRepackTypeId,
                remark: res.data.remark,
            });
        });
    },

    onPreButtonClick(e) {

    },

    onNextButtonClick(e) {

    },

    gotoPre() {
        wx.redirectTo({
            url: "./order_create_insurance",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./order_create_summary",
        });
    },
})
