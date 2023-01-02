using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core.Service;
using shunda.Domain.Fee;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsOrderPayExcelService {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;

    public LogisticsOrderPayExcelService(ISession session, CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    public byte[] Export(IList<LogisticsOrderPayInfo> orderPayList, IList<LogisticsOrderPayDateStatItem> dateStatList) {
        IWorkbook workbook = new XSSFWorkbook();

        var rowHeight = 22;
        var headerCenterCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook, HorizontalAlignment.Center);
        var contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        var contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        var dataFormat = workbook.CreateDataFormat();

        var precision0Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision0Style.DataFormat = dataFormat.GetFormat("0");

        var precision1Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision1Style.DataFormat = dataFormat.GetFormat("0.0");

        var precision2Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision2Style.DataFormat = dataFormat.GetFormat("0.00");

        var precision3Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision3Style.DataFormat = dataFormat.GetFormat("0.000");

        var precision6Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision6Style.DataFormat = dataFormat.GetFormat("0.000000");

        {
            var sheetName = "支付明细";
            var sheet = workbook.CreateSheet(sheetName);

            var rowIndex = 0;

            var row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            // 客户   订单   支付类型   支付宝/微信/PayPal单号   支付金额   操作用户   状态   完成时间

            var cellIndex = 0;
            sheet.SetColumnWidth(cellIndex, 16 * 256);
            var cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("客户");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("订单");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付类型");

            sheet.SetColumnWidth(cellIndex, 50 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付宝/微信/PayPal单号");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付金额");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("操作用户");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("状态");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("完成时间");

            foreach (var orderPay in orderPayList) {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;


                // 客户   订单   支付类型   支付宝/微信/PayPal单号   支付金额   操作用户   状态   完成时间

                // 客户
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                // cell.SetCellValue(orderPay.Customer?.CustomerCode);
                cell.SetCellValue(customerCodeService.GetCustomerCode(orderPay.Customer.Id));

                // 订单
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.Order?.Code);

                // 支付类型
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.PayType?.Name);

                // 支付宝/微信/PayPal单号 
                var outTradeNo = orderPay.OutTradeNo ?? string.Empty;
                if (string.IsNullOrWhiteSpace(orderPay.PayPalPaymentId) == false) outTradeNo = outTradeNo + ", " + orderPay.PayPalPaymentId;

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(outTradeNo);

                // 支付金额
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)orderPay.Amount);

                // 操作用户
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.User?.UserName);

                // 状态
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.Status?.Name);

                // 完成时间
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.UpdateTime.ToString("yyyy-MM-dd HH:mm"));
            }
        }

        {
            var sheetName = "按日期汇总";
            var sheet = workbook.CreateSheet(sheetName);

            var rowIndex = 0;

            var row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            var cellIndex = 0;
            sheet.SetColumnWidth(cellIndex, 16 * 256);
            var cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("日期");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付宝");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("微信");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("PayPal");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("余额");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("核销");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("总金额");


            foreach (var dateStat in dateStatList) {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(dateStat.Date.ToString("yyyy-MM-dd"));

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.AlipayAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.WechatAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.PaypalAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.BalanceAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.WriteOffAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.TotalAmount);
            }
        }

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.ToArray();
    }
}