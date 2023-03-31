using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerAoPinIdExcelService {
    private static readonly Regex REGEX_NUMBER = new Regex(@"[0-9]+", RegexOptions.Compiled);

    private readonly ISession session;

    public CustomerAoPinIdExcelService(ISession session) {
        this.session = session;
    }

    // 导入的Excel第一列是舜达ID，第二列是澳拼ID
    public void Import(IFormFile excelFile) {
        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        var headerRow = sheet.GetRow(0);
        if (headerRow == null) {
            throw new ValidationException("Excel第一行标题行数据不存在！");
        }

        var shundaHeaderCell = headerRow.GetCell(0);
        var aoPinHeaderCell = headerRow.GetCell(1);
        if (shundaHeaderCell == null || aoPinHeaderCell == null) {
            throw new ValidationException("Excel第一行第一列或第二列数据不存在！");
        }

        shundaHeaderCell.SetCellType(CellType.String);
        aoPinHeaderCell.SetCellType(CellType.String);

        string shundaHeaderCellValue = shundaHeaderCell.StringCellValue ?? string.Empty;
        string aoPinHeaderCellValue = aoPinHeaderCell.StringCellValue ?? string.Empty;

        if (shundaHeaderCellValue.Contains("舜达") == false || aoPinHeaderCellValue.Contains("舜达") == true) {
            throw new ValidationException("请确认数据第一列是舜达ID！");
        }

        if (shundaHeaderCellValue.Contains("澳拼") == true || aoPinHeaderCellValue.Contains("澳拼") == false) {
            throw new ValidationException("请确认数据第二列是澳拼ID！");
        }

        var customerAoPinIdImportList = new List<CustomerAoPinIdExcelImportItem>();
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
                if (cell == null) cell = row.CreateCell(cellIndex);

                cell.SetCellType(CellType.String);
            }

            cellIndex = 0;

            string shundaCheckinCodeStr = (row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(shundaCheckinCodeStr)) {
                break;
            }

            int checkinCodeNum = GetNumberFromStr(shundaCheckinCodeStr);
            string checkinCode = checkinCodeNum.ToString(); // 生成时已经保证不会生成以0开头的，所以直接 ToString() 就可以了

            string aoPinIdStr = (row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty).Trim();

            // 如果澳拼ID是空的暂时略过
            if (string.IsNullOrWhiteSpace(aoPinIdStr)) {
                break;
            }

            int aoPinId = GetNumberFromStr(aoPinIdStr);
            if (aoPinId == 0) {
                throw new ValidationException($"指定的澳拼ID#{aoPinIdStr}格式不正确！");
            }

            var customer = session.Query<CustomerInfo>()
                .Where(e => e.CheckinCode == checkinCode)
                .Take(1)
                .SingleOrDefault();
            if (customer == null) {
                throw new ValidationException($"指定的第{rowIndex}行客户#{shundaCheckinCodeStr}不存在！");
            }

            var customerAoPinId = new CustomerAoPinIdExcelImportItem {
                CustomerId = customer.Id,
                Customer = customer,
                AoPinId = aoPinId,
            };

            customerAoPinIdImportList.Add(customerAoPinId);
        }

        foreach (var customerAoPinId in customerAoPinIdImportList) {
            var customer = customerAoPinId.Customer;
            customer.AoPinId = customerAoPinId.AoPinId;

            session.Update(customer);
        }

        session.Flush();
    }

    private int GetNumberFromStr(string strValue) {
        // 解析数字部分，如果含有多个数字部分，取长度最长的数字部分
        MatchCollection matches = REGEX_NUMBER.Matches(strValue);

        string maxLengthNumberMatchValue = string.Empty;
        foreach (Match match in matches) {
            string matchValue = match.Value;
            if (matchValue.Length > maxLengthNumberMatchValue.Length) {
                maxLengthNumberMatchValue = matchValue;
            }
        }

        if (string.IsNullOrWhiteSpace(maxLengthNumberMatchValue)) {
            return 0;
        }

        if (int.TryParse(maxLengthNumberMatchValue, out int number) == false) {
            return 0;
        }

        return number;
    }
}