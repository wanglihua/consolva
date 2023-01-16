// pages/order_create/order_create_agreement.js
const app = getApp();

Page({
    data: {
        agreementList: [],
    },

    onLoad(options) {
        this.loadAgreementList();
    },

    loadAgreementList() {
        const that = this;
        app.post("/client/order/create/agreement/list", {}, function (res) {
            that.setData({
                agreementList: res.data.agreementList,
            });
        });
    },

    onAgreementCheckboxChange(e) {
        console.log(e);
    },

    onAgreementReadTap(e) {
        console.log(e);
    },

    onAgreementTitleTap(e) {
        console.log(e);
    },    

    onCreateOrderButtonClick(e) {

    }
})