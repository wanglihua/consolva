using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core.Service;
using shunda.Domain.Fee;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class FeeWriteOffExcelService {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;

    public FeeWriteOffExcelService(ISession session, CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    public byte[] Export(IList<LogisticsOrderPayInfo> feeWriteOffList) {
        foreach (LogisticsOrderPayInfo orderPay in feeWriteOffList) {
            orderPay.CustomerCodeCache = customerCodeService.GetCustomerCode(orderPay.Customer.Id);
        }

        IWorkbook workbook = new XSSFWorkbook();

        ISheet sheet = workbook.CreateSheet("费用核销");

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
        int cellIndex = 0;

        // row 
        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        cellIndex = 0;

        // 订单    客户    支付类型    支付金额    核销原因    操作用户    状态    生成时间   

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("订单");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("客户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("支付类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("支付金额");

        sheet.SetColumnWidth(cellIndex, 30 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("核销原因");

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
        cell.SetCellValue("生成时间");

        foreach (LogisticsOrderPayInfo orderPay in feeWriteOffList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 订单    客户    支付类型    支付金额    核销原因    操作用户    状态    生成时间    

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.Order?.Code);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.CustomerCodeCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.PayType?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision2Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue((double)orderPay.Amount);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.WriteOffReasonContent);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue($"{orderPay.User?.LoginName}({orderPay.User?.UserName})");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.Status?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.CreateTime.ToString("yyyy-MM-dd HH:mm"));
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.GetBuffer();
    }
}