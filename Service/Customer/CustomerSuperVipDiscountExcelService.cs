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
public class CustomerSuperVipDiscountExcelService {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;

    public CustomerSuperVipDiscountExcelService(ISession session,
        CustomerCodeService customerCodeService
    ) {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    public byte[] Export() {
        var sheetName = "超级VIP折扣";

        //NPOI.xs book = new NPOI.HSSF.UserModel.HSSFWorkbook();
        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet(sheetName);

        IList<CustomerSuperVipDiscountInfo> superVipDiscountList = session
            .Query<CustomerSuperVipDiscountInfo>()
            .OrderBy(e => e.Id)
            .ToList();

        var rowHeight = 22;
        var headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        var contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 客户 备注 折扣

        var rowIndex = 0;

        var row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        var cellIndex = 0;
        var cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("客户");

        cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("备注");

        cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("折扣%");

        foreach (var superVipDiscount in superVipDiscountList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 客户
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(superVipDiscount.Customer?.Id.ToString());

            // 备注
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(superVipDiscount.Description);

            // 折扣
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(superVipDiscount.Discount.ToString("#0.00"));
        }

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.GetBuffer();
    }

    public void Import(IFormFile excelFile, UserInfo loginUser) {
        if (excelFile == null) throw new ValidationException("导入失败，文件未上传！");

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) throw new ValidationException("导入的Excel文件应该至少有一个工作表！");

        IList<CustomerSuperVipDiscountInfo> superVipDiscountList = new List<CustomerSuperVipDiscountInfo>();

        // 客户  备注  折扣 

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

                cell?.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 客户
            var customerIdStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(customerIdStr)) break;

            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerIdStr);
            if (customerId == 0) {
                throw new ValidationException($"第{rowIndex}行数据 客户 数据格式有误，导入失败！");
            }

            var customer = session.Get<CustomerInfo>(customerId);
            if (customer == null) {
                throw new ValidationException($"第{rowIndex}行数据 客户 不存在，导入失败！");
            }

            // 备注
            var description = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            // 折扣
            var mallDiscountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(mallDiscountStr)) {
                throw new ValidationException($"第{rowIndex}行数据 折扣 不能为空，导入失败！");
            }

            if (decimal.TryParse(mallDiscountStr, out var discount) == false) {
                throw new ValidationException($"第{rowIndex}行数据 折扣 数据格式有误 不能为空，导入失败！");
            }


            var customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
            customerSuperVipDiscount.Customer = customer;
            customerSuperVipDiscount.Description = description;
            customerSuperVipDiscount.Discount = discount;
            customerSuperVipDiscount.User = loginUser;

            superVipDiscountList.Add(customerSuperVipDiscount);
        }

        foreach (var discount in superVipDiscountList) {
            var vipDiscountInDb = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == discount.Customer.Id)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscountInDb == null) {
                vipDiscountInDb = new CustomerSuperVipDiscountInfo();
                vipDiscountInDb.Customer = discount.Customer;
            }

            vipDiscountInDb.Discount = discount.Discount;

            session.SaveOrUpdate(discount);
        }

        session.Flush();
    }
}