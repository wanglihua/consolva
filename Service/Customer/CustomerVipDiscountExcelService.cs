using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerVipDiscountExcelService {
    private readonly ISession session;

    public CustomerVipDiscountExcelService(ISession session) {
        this.session = session;
    }

    public byte[] Export() {
        var sheetName = "VIP折扣设置";

        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet(sheetName);

        IList<CustomerVipDiscountInfo> vipDiscountList = session
            .Query<CustomerVipDiscountInfo>()
            .OrderBy(e => e.Id)
            .ToList();

        var rowHeight = 22;
        var headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        var contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 会员VIP等级    历史总积分    折扣

        var rowIndex = 0;

        var row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        var cellIndex = 0;
        var cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("会员VIP等级");

        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("历史总积分");

        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("折扣%");

        foreach (var vipDiscount in vipDiscountList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 会员VIP等级
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(vipDiscount.Rank.Name);

            // 历史总积分
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(vipDiscount.Score.ToString("#0.00"));

            // 折扣
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(vipDiscount.Discount.ToString("#0.00"));
        }

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.ToArray();
    }
}