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

    onRepackTypeButtonClick(e) {
        const repackType = e.currentTarget.dataset.item;
        const repackTypeId = repackType.id;
        const remark = this.data.remark;

        const that = this;
        app.post("/client/order/create/repack/type/save", {
            repackTypeId: repackTypeId,
            remark: remark,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.setData({
                selectRepackTypeId: repackTypeId,
            });
        });
    },

    onPreButtonClick(e) {
        const repackTypeId = this.data.selectRepackTypeId;
        const remark = this.data.remark;

        const that = this;
        app.post("/client/order/create/repack/type/save", {
            repackTypeId: repackTypeId,
            remark: remark,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.gotoPre();
        });
    },

    onNextButtonClick(e) {
        const selectRepackTypeId = this.data.selectRepackTypeId;
        if (selectRepackTypeId === 0) {
            wx.showModal({
                title: '错误',
                content: "请选择打包方式！",
                showCancel: false
            });
            return;
        }

        const repackTypeId = this.data.selectRepackTypeId;
        const remark = this.data.remark;

        const that = this;
        app.post("/client/order/create/repack/type/save", {
            repackTypeId: repackTypeId,
            remark: remark,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.gotoNext();
        });
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
