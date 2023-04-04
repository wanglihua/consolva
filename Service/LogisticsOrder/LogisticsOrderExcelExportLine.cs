namespace shunda.Service.LogisticsOrder;

public class LogisticsOrderExcelExportLine {
    public string OrderCode { get; set; } = string.Empty;

    public string CustomerCode { get; set; } = string.Empty;

    public string GoodsTypeName { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public string FirstModeName { get; set; } = string.Empty;

    public double OrderAmount { get; set; }

    public double TotalVolumeWeight { get; set; }

    public double TotalActualWeight { get; set; }

    public double TotalPackageCount { get; set; }

    public double MaxSideLength { get; set; }

    public string GoodsNames { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Consignee { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;
}