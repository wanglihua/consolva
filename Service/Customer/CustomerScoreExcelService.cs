using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerScoreExcelService {
    private readonly ISession session;

    public CustomerScoreExcelService(ISession session) {
        this.session = session;
    }

    public byte[] Export() {
        var customerList = session.Query<CustomerInfo>()
            .Where(e => e.Status == CustomerStatus.Normal)
            .OrderBy(e => e.Id)
            .ToList();

        IWorkbook workbook = new XSSFWorkbook();
        string sheetName = "客户积分";
        var sheet = workbook.CreateSheet(sheetName);

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

        var rowIndex = 0;

        var row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        var cellIndex = 0;
        var cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("客户");

        cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("有效积分");

        cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("历史积分");

        foreach (var customer in customerList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 客户
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(customer.CheckinCode);

            // 有效积分
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision2Style;
            cell.SetCellType(CellType.String);
            cell.SetCellValue((double)(customer.ValidScore));

            // 历史积分
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision2Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue((double)(customer.TotalScore));
        }

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.GetBuffer();
    }

    public void Import(IFormFile excelFile) {
    }
}