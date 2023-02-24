// pages/instruction/instruction_warehouse_address.js
const app = getApp();

Page({
    data: {
        address: {},
    },

    onLoad(options) {
        const that = this;
        app.post("/client/warehouse/address/detail", {}, function (res) {
            const address = res.data;
            that.setData({
                address: address,
            });
        });
    }
})

// /client/warehouse/address/detail

/*

using Newtonsoft.Json;

namespace shunda.Client.Areas.Instruction.Models.WarehouseAddress;

public class WarehouseAddressDetailModel {
    [JsonProperty("customerCode")]
    public string CustomerCode { get; set; } = string.Empty;

    [JsonProperty("area")]
    public string Area { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonProperty("zipcode")]
    public string Zipcode { get; set; } = string.Empty;

    [JsonProperty("fullAddress")]
    public string FullAddress { get; set; } = string.Empty;
}


 */