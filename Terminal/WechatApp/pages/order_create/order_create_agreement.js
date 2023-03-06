const app = getApp();

Page({
    data: {
        agreementList: [],
        detailAgreement: {},
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
        const agreement = e.currentTarget.dataset.item;

        wx.navigateTo({
            url: "./order_create_web_view?url=" + agreement.url + "&title=" + agreement.title,
        });
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
        let allAccepted = true;
        const agreementList = this.data.agreementList;
        const noMoreOrderActionChecked = this.data.noMoreOrderActionChecked;
        const transferAfterPaidChecked = this.data.transferAfterPaidChecked;

        for (const agreement of agreementList) {
            if (agreement.checked !== true) {
                allAccepted = false;
            }
        }

        if (noMoreOrderActionChecked !== true || transferAfterPaidChecked !== true) {
            allAccepted = false;
        }

        if (allAccepted !== true) {
            wx.showModal({
                title: '转运协议',
                content: '请先阅读并同意所有转运协议！',
                showCancel: false
            });
            return;
        }

        // 创建订单
        app.post("/client/order/create/new", {}, function (res) {
            const orderId = res.data.orderId;
            // 导航到订单详细界面
            wx.redirectTo({
                url: "/pages/order/order_detail?id=" + orderId,
            });
        });
    }
})