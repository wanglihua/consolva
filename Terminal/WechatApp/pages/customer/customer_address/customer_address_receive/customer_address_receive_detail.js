// pages/customer/customer_address/customer_address_receive/customer_address_receive_detail.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        id: 0,
        countryId: 0,
        firstName: '',
        lastName: '',
        mobile: '',
        address: '',
        city: '',
        state: '',
        zipcode: '',
        tfn: '',
        company: '',
        unitLoc: '',
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/customer/receive/address/detail", {
            id: id,
        }, function (res) {
            that.setData({
                id: res.data.id,
                countryId: res.data.countryId,
                firstName: res.data.firstName,
                lastName: res.data.lastName,
                mobile: res.data.mobile,
                address: res.data.address,
                city: res.data.city,
                state: res.data.state,
                zipcode: res.data.zipcode,
                tfn: res.data.tfn,
                company: res.data.company,
                unitLoc: res.data.unitLoc,
            });
        });
    },
    onSaveClick(e) {
        console.log(e);
    },
    onCancelClick(e) {
        console.log(e);
    }
})