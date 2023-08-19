using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core.Service;
using shunda.Domain.Customer;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerPromotionExcelService {
    private readonly CustomerCodeService customerCodeService;

    public CustomerPromotionExcelService(CustomerCodeService customerCodeService) {
        this.customerCodeService = customerCodeService;
    }

    public byte[] GetExcelContent(IList<CustomerPromotionInfo> customerPromotionList) {
        IWorkbook workbook = new XSSFWorkbook();

        string sheetName = "会员推广";
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

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("上家会员");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("下家会员ID");

        sheet.SetColumnWidth(cellIndex, 40 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("下家OpenID");

        sheet.SetColumnWidth(cellIndex, 20 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("关注时间");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("下家支付总额");

        if (customerPromotionList != null && customerPromotionList.Count > 0) {
            foreach (var promotion in customerPromotionList) {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                string parentCustomerCode = customerCodeService.GetCustomerCode(promotion.Parent.Id);
                cell.SetCellValue(parentCustomerCode);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);

                string childCustomerCode = string.Empty;
                if (promotion.Child != null) {
                    customerCodeService.GetCustomerCode(promotion.Child.Id);
                }

                cell.SetCellValue(childCustomerCode);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(promotion.OpenId);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(promotion.CreateTime.ToString("yyyy-MM-dd HH:mm"));


                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)(promotion.ChildPayAmount ?? 0));
            }
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.ToArray();
    }
}