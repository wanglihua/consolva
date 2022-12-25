// pages/customer/customer_address/customer_address_receive/customer_address_receive_list.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        receiveAddressList: [],
        receiveAddressCount: 0,
        sendAddressCount: 0,

        showActionSheet: false,
        actionSheetActions: [],
        currentAddress: {},
    },

    onLoad(options) {
        this.loadReceiveAddressList();
    },

    loadReceiveAddressList() {
        const that = this;
        app.post("/client/customer/receive/address/list", {}, function (res) {
            const receiveAddressList = res.data.receiveAddressList;
            const receiveAddressCount = res.data.receiveAddressCount;
            const sendAddressCount = res.data.sendAddressCount;

            that.fillListData(receiveAddressList);

            that.setData({
                receiveAddressList: receiveAddressList,
                receiveAddressCount: receiveAddressCount,
                sendAddressCount: sendAddressCount,
            });
        });
    },

    fillListData(receiveAddressList) {
        for (const address of receiveAddressList) {
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
        // console.log(e.detail.name);
        if (e.detail.name === "send") {
            wx.redirectTo({
                url: "../customer_address_send/customer_address_send_list",
            });
        }
    },

    onAddCustomerReceiveAddress(e) {
        // console.log(e);
        wx.navigateTo({
            url: "./customer_address_receive_detail?id=0",
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
        const addressId = this.data.currentAddress.id;
        const action = e.detail.action;

        // console.log(addressId);
        // console.log(action);

        const that = this;

        if (action === "setDefault") {
            app.post("/client/customer/receive/address/set/default", {
                id: addressId,
            }, function (res) {
                wx.showToast({
                    title: res.message,
                });

                const receiveAddressList = res.data.receiveAddressList;
                const receiveAddressCount = res.data.receiveAddressCount;
                const sendAddressCount = res.data.sendAddressCount;

                that.fillListData(receiveAddressList);

                that.setData({
                    receiveAddressList: receiveAddressList,
                    receiveAddressCount: receiveAddressCount,
                    sendAddressCount: sendAddressCount,
                });
            });
        }

        if (action === "edit") {
            wx.navigateTo({
                url: './customer_address_receive_detail?id=' + addressId,
            });
        }

        if (action === "delete") {

        }

        this.setData({
            showActionSheet: false,
        });
    }
})