// pages/order_create/order_create_agreement.js
const app = getApp();

Page({
    data: {
        agreementList: [],
        noMoreOrderActionChecked: false,
        transferAfterPaidChecked: false,
        allCheckChecked: false,
    },

    onLoad(options) {
        this.loadAgreementList();
    },

    loadAgreementList() {
        const that = this;
        app.post("/client/order/create/agreement/list", {}, function (res) {
            const agreementList = res.data.agreementList;
            for (const agreement of agreementList) {
                agreement.checked = false;
            }

            that.setData({
                agreementList: agreementList,
            });
        });
    },

    onAgreementCheckboxChange(e) {
        const agreementId = e.currentTarget.dataset.item.id;
        const agreementList = this.data.agreementList;
        for (const agreement of agreementList) {
            if (agreement.id === agreementId) {
                agreement.checked = !agreement.checked;
            }
        }

        this.setData({
            agreementList: agreementList,
        });
    },

    onAgreementReadTap(e) {
        const agreementId = e.currentTarget.dataset.item.id;
        const agreementList = this.data.agreementList;
        for (const agreement of agreementList) {
            if (agreement.id === agreementId) {
                agreement.checked = !agreement.checked;
            }
        }

        this.setData({
            agreementList: agreementList,
        });
    },

    onAgreementTitleTap(e) {
        console.log(e);
        const agreementId = e.currentTarget.dataset.item.id;
    },

    onNoMoreOrderActionCheckboxChange(e) {
        const noMoreOrderActionChecked = this.data.noMoreOrderActionChecked;
        this.setData({
            noMoreOrderActionChecked: !noMoreOrderActionChecked,
        });
    },

    onNoMoreOrderActionTitleTap(e) {
        const noMoreOrderActionChecked = this.data.noMoreOrderActionChecked;
        this.setData({
            noMoreOrderActionChecked: !noMoreOrderActionChecked,
        });
    },

    onTransferAfterPaidCheckboxChange(e) {
        const transferAfterPaidChecked = this.data.transferAfterPaidChecked;
        this.setData({
            transferAfterPaidChecked: !transferAfterPaidChecked,
        });
    },

    onTransferAfterPaidTitleTap(e) {
        const transferAfterPaidChecked = this.data.transferAfterPaidChecked;
        this.setData({
            transferAfterPaidChecked: !transferAfterPaidChecked,
        });
    },

    onAllCheckCheckboxChange(e) {
        const allCheckChecked = !this.data.allCheckChecked;
        const agreementList = this.data.agreementList;

        for (const agreement of agreementList) {
            agreement.checked = allCheckChecked;
        }

        this.setData({
            agreementList: agreementList,
            noMoreOrderActionChecked: allCheckChecked,
            transferAfterPaidChecked: allCheckChecked,
            allCheckChecked: allCheckChecked,
        });
    },

    onAllCheckTitleTap(e) {
        const allCheckChecked = !this.data.allCheckChecked;
        const agreementList = this.data.agreementList;

        for (const agreement of agreementList) {
            agreement.checked = allCheckChecked;
        }

        this.setData({
            agreementList: agreementList,
            noMoreOrderActionChecked: allCheckChecked,
            transferAfterPaidChecked: allCheckChecked,
            allCheckChecked: allCheckChecked,
        });
    },

    onCreateOrderButtonClick(e) {

    }
})