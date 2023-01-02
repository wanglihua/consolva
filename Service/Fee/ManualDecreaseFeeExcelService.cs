using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core.Service;
using shunda.Domain.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class ManualDecreaseFeeExcelService {
    private readonly ISession session;

    public ManualDecreaseFeeExcelService(ISession session) {
        this.session = session;
    }

    public byte[] Export(IList<LogisticsOrderFeeInfo> feeList) {
        IWorkbook workbook = new XSSFWorkbook();

        ISheet sheet = workbook.CreateSheet("减少费用");

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

        // 客户 订单 费用类型 金额 减少费用原因 描述 操作用户 生成时间

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
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
        cell.SetCellValue("费用类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("金额");

        sheet.SetColumnWidth(cellIndex, 50 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("减少费用原因");

        sheet.SetColumnWidth(cellIndex, 50 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("描述");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("操作用户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("生成时间");

        foreach (LogisticsOrderFeeInfo fee in feeList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 客户 订单 费用类型 金额 减少费用原因 描述 操作用户 生成时间    

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(fee.CustomerCodeCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(fee.Order?.Code);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(fee.Type?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision2Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue((double)fee.Fee);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(fee.ManualReasonContent);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(fee.Description);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue($"{fee.ManualUser?.LoginName}({fee.ManualUser?.UserName})");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(fee.CreateTime.ToString("yyyy-MM-dd HH:mm"));
        }


        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.ToArray();
    }
}