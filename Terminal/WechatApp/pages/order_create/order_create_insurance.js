// pages/order_create/order_create_insurance.js
const app = getApp();

Page({
    data: {
        insuranceList: [],
        selectInsuranceId: 0,

        showDetail: false,
        detailInsurance: {},
    },

    onLoad(options) {
        this.loadInsuranceList();
    },

    loadInsuranceList() {
        const that = this;
        app.post("/client/order/create/insurance/list", {}, function (res) {
            that.setData({
                insuranceList: res.data.insuranceList,
                selectInsuranceId: res.data.selectInsuranceId,
            });
        })
    },

    onInsuranceButtonClick(e) {
        const insurance = e.currentTarget.dataset.item;
        const insuranceId = insurance.id;

        const that = this;
        app.post("/client/order/create/insurance/save", {
            insuranceId: insuranceId,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.setData({
                insuranceId: insuranceId,
            });

            that.gotoNext();
        });
    },

    onViewDetailClick(e) {
        const that = this;

        that.setData({
            showDetail: true,
        });
    },
    
    onDetailClose(e) {
        this.setData({
            showDetail: false,
        });
    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {
        const selectInsuranceId = this.data.selectInsuranceId;
        if (selectInsuranceId === 0) {
            wx.showModal({
                title: '错误',
                content: "请选择保险方案！",
                showCancel: false
            });
            return;
        }

        this.gotoNext();
    },

    gotoPre() {
        wx.redirectTo({
            url: "./order_create_send_address",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./order_create_summary",
        });
    }
})
