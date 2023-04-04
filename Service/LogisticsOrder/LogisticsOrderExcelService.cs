using NHibernate.Linq;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderExcelService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsOrderExcelService(ISession session) {
        this.session = session;
    }

    public byte[] Export(IList<LogisticsOrderInfo> orderList) {
        if (orderList == null || orderList.Count == 0) {
            throw new ValidationException("订单列表不能为空！");
        }

        IList<int> orderIdList = orderList.Select(e => e.Id).ToList();

        // 收货地址
        var orderReceiveAddressList = session.Query<LogisticsOrderReceiveAddressInfo>()
            .Where(e => orderIdList.Contains(e.Order.Id))
            .ToList();

        // 快递单列表
        var expressList = session.Query<ExpressInfo>()
            .Where(e => orderIdList.Contains(e.Order!.Id))
            .ToList();

        // 物品列表
        var expressIdList = expressList.Select(e => e.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        // 打包数据
        var packDataList = session.Query<LogisticsOrderPackDataInfo>()
            .Where(e => orderIdList.Contains(e.Order.Id))
            .ToList();

        // 头程列表, 优先取打包物流方式
        var orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Fetch(e => e.FirstMode)
            .Where(e => orderIdList.Contains(e.Order.Id))
            .ToList();

        IWorkbook workbook = new XSSFWorkbook();

        {
            string sheetName = "订单列表";
            ISheet sheet = workbook.CreateSheet(sheetName);

            int rowHeight = 22;
            ICellStyle headerCenterCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook, HorizontalAlignment.Center);
            ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
            ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

            IDataFormat dataFormat = workbook.CreateDataFormat();

            ICellStyle precision0Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
            precision0Style.DataFormat = dataFormat.GetFormat("0");

            ICellStyle precision1Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
            precision1Style.DataFormat = dataFormat.GetFormat("0.0");

            ICellStyle precision2Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
            precision2Style.DataFormat = dataFormat.GetFormat("0.00");

            ICellStyle precision3Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
            precision3Style.DataFormat = dataFormat.GetFormat("0.000");

            ICellStyle precision6Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
            precision6Style.DataFormat = dataFormat.GetFormat("0.000000");

            int rowIndex = 0;

            IRow row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            int cellIndex = 0;

            sheet.SetColumnWidth(cellIndex, 28 * 256);
            ICell cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("订单编号");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("客户ID");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("产品类型");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("国家");

            sheet.SetColumnWidth(cellIndex, 100 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("头程方式");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("费用");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("体积重");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("实际重");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("件数");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("最长边");

            sheet.SetColumnWidth(cellIndex, 150 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("物品信息");

            sheet.SetColumnWidth(cellIndex, 48 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("地址");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("联系人");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("电话");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("公司/店铺名");

            foreach (LogisticsOrderInfo order in orderList) {
                int orderId = order.Id;
                var orderReceiveAddress = orderReceiveAddressList
                    .Where(e => e.Order.Id == orderId)
                    .Take(1)
                    .SingleOrDefault();

                if (orderReceiveAddress == null) {
                    throw new Exception($"订单#{order.Code}收货的地址不能为空！");
                }

                string address = $"{orderReceiveAddress.Address}, {orderReceiveAddress.City} {orderReceiveAddress.State} {orderReceiveAddress.Zipcode}";

                if (string.IsNullOrWhiteSpace(orderReceiveAddress.UnitLoc) == false) {
                    address = $"{orderReceiveAddress.UnitLoc}, " + address;
                }

                string consignee = orderReceiveAddress.FirstName;
                if (string.IsNullOrWhiteSpace(orderReceiveAddress.LastName) == false) {
                    consignee = consignee + " " + orderReceiveAddress.LastName;
                }

                var thisOrderFirstModeList = orderFirstModeList.Where(e => e.Order.Id == orderId).ToList();

                var lineList = new List<LogisticsOrderExcelExportLine>();

                if (thisOrderFirstModeList.Count == 0) {
                    var packData = packDataList.Where(e => e.Order.Id == orderId).Take(1).SingleOrDefault();
                    if (packData == null) {
                        packData = new LogisticsOrderPackDataInfo();
                    }

                    var orderExpressList = expressList.Where(e => e.Order!.Id == orderId).ToList();

                    var orderExpressIdList = orderExpressList.Select(e => e.Id).ToList();
                    var orderExpressGoodsList = expressGoodsList.Where(e => orderExpressIdList.Contains(e.Express.Id)).ToList();

                    LogisticsOrderExcelExportLine line = new LogisticsOrderExcelExportLine {
                        OrderCode = order.Code,
                        CustomerCode = order.CustomerCode,
                        GoodsTypeName = order.GoodsType.Name,
                        CountryName = order.Country.Name,
                        FirstModeName = $"{order.FirstMode.Code}({order.FirstMode.Name})",
                        OrderAmount = (double)order.OrderAmount,
                        TotalVolumeWeight = (double)(packData.TotalVolumeWeight),
                        TotalActualWeight = (double)(packData.TotalActualWeight),
                        TotalPackageCount = (double)(packData.TotalPackageCount),
                        MaxSideLength = (double)(packData.MaxSideLength),
                        GoodsNames = string.Join(",", orderExpressGoodsList.Select(e => $"{e.Goods.Name}{e.Quantity}{e.Unit}").Distinct()),
                        Address = address,
                        Consignee = consignee,
                        Mobile = orderReceiveAddress.Mobile,
                        Company = orderReceiveAddress.Company,
                    };

                    lineList.Add(line);
                } else {
                    bool first = true;
                    foreach (var ofm in thisOrderFirstModeList) {
                        double orderAmount = 0d;
                        if (first == true) {
                            orderAmount = (double)order.OrderAmount;
                        }

                        LogisticsOrderExcelExportLine line = new LogisticsOrderExcelExportLine {
                            OrderCode = order.Code,
                            CustomerCode = order.CustomerCode,
                            GoodsTypeName = order.GoodsType.Name,
                            CountryName = order.Country.Name,
                            FirstModeName = $"{ofm.FirstMode.Code}({ofm.FirstMode.Name})",

                            OrderAmount = orderAmount,
                            TotalVolumeWeight = 0d,
                            TotalActualWeight = 0d,
                            TotalPackageCount = 0d,
                            MaxSideLength = 0d,
                            GoodsNames = "",

                            Address = address,
                            Consignee = consignee,
                            Mobile = orderReceiveAddress.Mobile,
                            Company = orderReceiveAddress.Company,
                        };

                        lineList.Add(line);

                        if (first == true) {
                            first = false;
                        }
                    }
                }

                foreach (var line in lineList) {
                    row = sheet.CreateRow(rowIndex++);
                    row.HeightInPoints = rowHeight;

                    cellIndex = 0;

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.OrderCode);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.CustomerCode);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.GoodsTypeName);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.CountryName);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.FirstModeName);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = precision3Style;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(line.OrderAmount);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = precision3Style;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(line.TotalVolumeWeight);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = precision3Style;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(line.TotalActualWeight);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = precision0Style;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(line.TotalPackageCount);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = precision2Style;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(line.MaxSideLength);

                    // 物品信息
                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.GoodsNames);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.Address);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.Consignee);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.Mobile);

                    cell = row.CreateCell(cellIndex++);
                    cell.CellStyle = contentCenterCellStyle;
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(line.Company);
                }
            }
        }

        {
            string sheetName = "订单地址";
            ISheet sheet = workbook.CreateSheet(sheetName);

            int rowHeight = 22;
            ICellStyle headerCenterCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook, HorizontalAlignment.Center);
            ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
            ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

            int rowIndex = 0;

            IRow row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            int cellIndex = 0;

            sheet.SetColumnWidth(cellIndex, 28 * 256);
            ICell cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("订单编号");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("客户ID");

            // 收货人First Name
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("收货人First Name");

            // 收货人Last Name
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("收货人Last Name");

            // 手机号码
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("手机号码");

            // Unit号
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("Unit号");

            // 门牌号/路名
            sheet.SetColumnWidth(cellIndex, 35 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("门牌号/路名");

            // 区/Suburb
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("区/Suburb");

            // 州
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("州");

            // 邮编
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("邮编");

            // ABN/ACN
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("ABN/ACN");

            // 公司
            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("公司");


            foreach (LogisticsOrderInfo order in orderList) {
                int orderId = order.Id;
                var orderReceiveAddress = orderReceiveAddressList
                    .Where(e => e.Order.Id == orderId)
                    .Take(1)
                    .SingleOrDefault();

                if (orderReceiveAddress == null) {
                    throw new Exception($"订单#{order.Code}收货的地址不能为空！");
                }

                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(order.Code);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(order.CustomerCode);

                // 收货人First Name
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.FirstName);

                // 收货人Last Name
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.LastName);

                // 手机号码
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.Mobile);

                // Unit号
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.UnitLoc);

                // 门牌号/路名
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.Address);

                // 区/Suburb
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.City);

                // 州
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.State);

                // 邮编
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.Zipcode);

                // ABN/ACN
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.Tfn);

                // 公司
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.Company);
            }
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.ToArray();
    }
}