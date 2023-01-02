using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Pass;
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

    public void Import(IFormFile excelFile, UserInfo loginUser) {
        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<CustomerVipDiscountInfo> vipDiscountList = new List<CustomerVipDiscountInfo>();

        // 略过标题行，所以 rowIndex = 1 开始
        var lastRowNum = sheet.LastRowNum;
        for (var rowIndex = 1; rowIndex <= lastRowNum; rowIndex++) {
            var row = sheet.GetRow(rowIndex);

            if (row == null) break;

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            var cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++) {
                var cell = row.GetCell(cellIndex);
                if (cell == null) cell = row.CreateCell(cellIndex);

                cell.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 会员VIP等级
            var rankName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rankName)) break;

            var rank = CustomerVipRank.GetByName(rankName);
            if (rank == CustomerVipRank.None) {
                throw new ValidationException($"第{rowIndex}行数据 会员VIP等级 为空或数据不合法，导入失败！");
            }

            // 历史总积分
            var scoreStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(scoreStr)) {
                throw new ValidationException($"第{rowIndex}行数据 历史总积分 不能为空，导入失败！");
            }

            if (decimal.TryParse(scoreStr, out var score) == false) {
                throw new ValidationException($"第{rowIndex}行数据 历史总积分 数据格式有误，导入失败！");
            }

            // 折扣
            var discountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(discountStr)) {
                throw new ValidationException($"第{rowIndex}行数据 折扣 不能为空，导入失败！");
            }

            if (decimal.TryParse(discountStr, out var Discount) == false) {
                throw new ValidationException($"第{rowIndex}行数据 折扣 数据格式有误，导入失败！");
            }

            var vipDiscount = new CustomerVipDiscountInfo();
            vipDiscount.Rank = rank;
            vipDiscount.Score = score;
            vipDiscount.Discount = Discount;
            vipDiscount.User = loginUser;

            vipDiscountList.Add(vipDiscount);
        }


        foreach (var vipDiscount in vipDiscountList) {
            var vipDiscountInDb = session.Query<CustomerVipDiscountInfo>()
                .Where(e => e.Rank == vipDiscount.Rank)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscountInDb == null) {
                vipDiscountInDb = new CustomerVipDiscountInfo();
                vipDiscountInDb.Rank = vipDiscount.Rank;
            }

            vipDiscountInDb.Score = vipDiscount.Score;
            vipDiscountInDb.Discount = vipDiscount.Discount;

            session.SaveOrUpdate(vipDiscount);
        }

        session.Flush();
    }
}