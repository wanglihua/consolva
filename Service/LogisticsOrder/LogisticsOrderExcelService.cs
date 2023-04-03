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
        // 导出的时候，优先实际打包的物流方式，还没打包时取订单的创建物流方式

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

        // 仓位列表
        var storageLocIdList = expressList.Select(e => e.StorageLoc.Id).ToList();
        var storageLocList = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Where(e => storageLocIdList.Contains(e.Id))
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
            cell.SetCellValue("目的仓库");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("所在仓位");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("仓位数量");

            sheet.SetColumnWidth(cellIndex, 20 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("快递单数量");

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

                var packData = packDataList.Where(e => e.Order.Id == orderId).Take(1).SingleOrDefault();
                if (packData == null) {
                    packData = new LogisticsOrderPackDataInfo();
                }

                var orderExpressList = expressList.Where(e => e.Order!.Id == orderId).ToList();
                var orderStorageLocIdList = orderExpressList.Select(e => e.StorageLoc.Id).ToList();
                var orderStorageLocList = storageLocList.Where(e => orderStorageLocIdList.Contains(e.Id)).ToList();

                var orderExpressIdList = orderExpressList.Select(e => e.Id).ToList();
                var orderExpressGoodsList = expressGoodsList.Where(e => orderExpressIdList.Contains(e.Express.Id)).ToList();

                var thisOrderFirstModeList = orderFirstModeList.Where(e => orderIdList.Contains(e.Order.Id)).ToList();

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

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(order.GoodsType?.Name);


                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(string.Join(",", orderStorageLocList.Select(e => e.GetFullName()).OrderBy(e => e).ToList()));

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision0Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(orderStorageLocList.Count);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision0Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(orderExpressList.Count);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);

                if (thisOrderFirstModeList.Count == 0) {
                    cell.SetCellValue($"{order.FirstMode.Code}({order.FirstMode.Name})");
                } else {
                    cell.SetCellValue(string.Join(",", thisOrderFirstModeList.Select(e => $"{e.FirstMode.Code}({e.FirstMode.Name})")));
                }

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision3Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)order.OrderAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision3Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)(packData.TotalVolumeWeight));

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision3Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)(packData.TotalActualWeight));

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision0Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)(packData.TotalPackageCount));

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)(packData.MaxSideLength));

                // 物品信息
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(string.Join(",", orderExpressGoodsList.Select(e => $"{e.Goods.Name}{e.Quantity}{e.Unit}").Distinct()));

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);

                string address = $"{orderReceiveAddress.Address}, {orderReceiveAddress.City} {orderReceiveAddress.State} {orderReceiveAddress.Zipcode}";

                if (string.IsNullOrWhiteSpace(orderReceiveAddress.UnitLoc) == false) {
                    address = $"{orderReceiveAddress.UnitLoc}, " + address;
                }

                cell.SetCellValue(address);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);

                string consignee = orderReceiveAddress.FirstName;
                if (string.IsNullOrWhiteSpace(orderReceiveAddress.LastName) == false) {
                    consignee = consignee + " " + orderReceiveAddress.LastName;
                }

                cell.SetCellValue(consignee);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.Mobile);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderReceiveAddress.Company);
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