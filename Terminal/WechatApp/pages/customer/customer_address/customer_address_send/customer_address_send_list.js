// pages/customer/customer_address/customer_address_send/customer_address_send_list.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        sendAddressList: [],
        sendAddressCount: 0,
        receiveAddressCount: 0,

        showActionSheet: false,
        actionSheetActions: [],
        currentAddress: {},
    },

    onLoad(options) {
        this.loadSendAddressList();
    },

    loadSendAddressList() {
        const that = this;
        app.post("/client/customer/send/address/list", {}, function (res) {
            const sendAddressList = res.data.sendAddressList;
            const sendAddressCount = res.data.sendAddressCount;
            const receiveAddressCount = res.data.receiveAddressCount;

            that.fillListData(sendAddressList);

            that.setData({
                sendAddressList: sendAddressList,
                sendAddressCount: sendAddressCount,
                receiveAddressCount: receiveAddressCount,
            });
        });
    },

    fillListData(sendAddressList) {
        for (const address of sendAddressList) {
            if (address.isDefault) {
                address.defaultStr = '默认地址';
                address.actions = "编辑，删除";
            } else {
                address.defaultStr = '';
                address.actions = "设为默认，编辑，删除";
            }
        }
    },

    onTabClick(e) {
        if (e.detail.name === "receive") {
            wx.redirectTo({
                url: "../customer_address_receive/customer_address_receive_list",
            });
        }
    },

    onAddCustomerSendAddress(e) {
        wx.navigateTo({
            url: "./customer_address_send_detail?id=0",
        });
    },

    onActionCellClick(e) {
        const currentAddress = e.currentTarget.dataset.item;

        const actionSheetActions = [];

        let setDefaultDisabled = false;
        if (currentAddress.isDefault === true) {
            setDefaultDisabled = true;
        }

        actionSheetActions.push({
            action: "setDefault",
            name: '设为默认',
            className: 'iconfont iconfont-use-agreement',
            disabled: setDefaultDisabled,
        });

        actionSheetActions.push({
            action: "edit",
            name: '编辑',
            className: 'iconfont iconfont-edit',
        });

        actionSheetActions.push({
            action: "delete",
            name: '删除',
            className: 'iconfont iconfont-delete',
        });

        this.setData({
            currentAddress: currentAddress,
            actionSheetActions: actionSheetActions,
            showActionSheet: true
        });
    },
    onActionSheetCloseClick(e) {
        this.setData({
            showActionSheet: false
        });
    },
    onActionSheetSelect(e) {
        const currentAddress = this.data.currentAddress;
        const addressId = currentAddress.id;
        const name = currentAddress.name;
        const action = e.detail.action;

        const that = this;

        if (action === "setDefault") {
            app.post("/client/customer/send/address/set/default", {
                id: addressId,
            }, function (res) {
                wx.showToast({
                    title: res.message,
                });

                const sendAddressList = res.data.sendAddressList;
                const sendAddressCount = res.data.sendAddressCount;
                const receiveAddressCount = res.data.receiveAddressCount;

                that.fillListData(sendAddressList);

                that.setData({
                    sendAddressList: sendAddressList,
                    sendAddressCount: sendAddressCount,
                    receiveAddressCount: receiveAddressCount,
                });
            });
        }

        if (action === "edit") {
            wx.navigateTo({
                url: './customer_address_send_detail?id=' + addressId,
            });
        }

        if (action === "delete") {
            wx.showModal({
                title: '删除确认',
                content: '确实要删除 ' + name + ' 地址吗？',
                success(res) {
                    if (res.confirm) {
                        app.post("/client/customer/send/address/delete", {
                            id: addressId,
                        }, function (res) {
                            wx.showToast({
                                title: res.message,
                            });

                            that.setData({
                                showActionSheet: false
                            });

                            // 立即设置列表数据
                            const sendAddressList = res.data.sendAddressList;
                            const sendAddressCount = res.data.sendAddressCount;
                            const receiveAddressCount = res.data.receiveAddressCount;

                            that.fillListData(sendAddressList);

                            that.setData({
                                sendAddressList: sendAddressList,
                                sendAddressCount: sendAddressCount,
                                receiveAddressCount: receiveAddressCount,
                            });
                        });
                    }

                    if (res.cancel) {
                        that.setData({
                            showActionSheet: false
                        });
                    }
                }
            });
        }

        this.setData({
            showActionSheet: false,
        });
    }
})