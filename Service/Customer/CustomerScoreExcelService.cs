using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerScoreExcelService {
    private readonly ISession session;
    private readonly CustomerVipDiscountService customerVipDiscountService;

    public CustomerScoreExcelService(ISession session, CustomerVipDiscountService customerVipDiscountService) {
        this.session = session;
        this.customerVipDiscountService = customerVipDiscountService;
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
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        var customerListImport = new List<CustomerInfo>();

        // 略过标题行，所以 rowIndex = 1 开始
        var lastRowNum = sheet.LastRowNum;
        for (var rowIndex = 1; rowIndex <= lastRowNum; rowIndex++) {
            var row = sheet.GetRow(rowIndex);

            if (row == null) {
                break;
            }

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            var cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++) {
                var cell = row.GetCell(cellIndex);
                if (cell == null) {
                    cell = row.CreateCell(cellIndex);
                }

                cell.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 客户
            var checkinCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(checkinCode)) {
                break;
            }

            checkinCode = checkinCode.Trim();

            var customer = session.Query<CustomerInfo>()
                .Where(e => e.CheckinCode == checkinCode)
                .Take(1)
                .SingleOrDefault();

            if (customer == null) {
                throw new ValidationException($"第{rowIndex}行数据 客户 不存在，导入失败！");
            }

            // 有效积分
            var validScoreStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(validScoreStr)) {
                throw new ValidationException($"第{rowIndex}行数据 有效积分 不能为空，导入失败！");
            }

            if (decimal.TryParse(validScoreStr, out var validScore) == false) {
                throw new ValidationException($"第{rowIndex}行数据 有效积分 数据格式有误 不能为空，导入失败！");
            }

            // 历史积分
            var totalScoreStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(totalScoreStr)) {
                throw new ValidationException($"第{rowIndex}行数据 历史积分 不能为空，导入失败！");
            }

            if (decimal.TryParse(totalScoreStr, out var totalScore) == false) {
                throw new ValidationException($"第{rowIndex}行数据 历史积分 数据格式有误 不能为空，导入失败！");
            }

            customer.ValidScore = validScore;
            customer.TotalScore = totalScore;

            customerListImport.Add(customer);
        }

        foreach (var customer in customerListImport) {
            // 其实这里不调用 update，也会保存到数据库的
            session.Update(customer);

            session.Save(new CustomerScoreLogInfo {
                Customer = customer,
                ScoreType = CustomerScoreType.Import,
                Score = customer.ValidScore,
                Amount = 0,
                IsValidScore = true,
                LogisticsOrderPay = null,
                LogisticsOrder = null,
                LogisticsOrderCode = string.Empty,
                LogisticsOrderCustomer = null,
                Description = "后台导入有效积分"
            });

            session.Save(new CustomerScoreLogInfo {
                Customer = customer,
                ScoreType = CustomerScoreType.Import,
                Score = customer.TotalScore,
                Amount = 0,
                IsValidScore = false,
                LogisticsOrderPay = null,
                LogisticsOrder = null,
                LogisticsOrderCode = string.Empty,
                LogisticsOrderCustomer = null,
                Description = "后台导入历史积分"
            });

            session.Flush();
        }

        customerVipDiscountService.UpdateAllCustomerVip();

        session.Flush();
    }
}