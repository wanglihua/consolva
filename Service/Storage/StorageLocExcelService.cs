using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Service.Storage;

[Service(ServiceType.Scoped)]
public class StorageLocExcelService {
    private readonly ISession session;

    public StorageLocExcelService(ISession session) {
        this.session = session;
    }

    public byte[] ExportOffShelfOrder(int shelfId) {
        var shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null) {
            throw new ValidationException($"指定的货架{shelfId}，不存在！");
        }

        var sheetName = "仓位下架顺序";

        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId && e.Status == StorageLocStatus.Enabled)
            .OrderBy(e => e.RowNum).ThenBy(e => e.ColNum)
            .ToList();

        IWorkbook workbook = new XSSFWorkbook();
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

        // 层 列 下架顺序

        var cellIndex = 0;
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        var cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("层");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("列");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("下架顺序");

        foreach (var sl in storageLocList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 层
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(sl.RowNum);

            // 列
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(sl.ColNum);

            // 下架顺序
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(sl.OffShelfOrderNum);
        }

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.GetBuffer();
    }

    public void ImportOffShelfOrder(int shelfId, IFormFile excelFile) {
        var shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null) {
            throw new ValidationException($"指定的货架{shelfId}，不存在！");
        }

        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<StorageLocInfo> storageLocListInDb = session.Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId)
            .ToList();

        IList<StorageLocInfo> storageLocListFromExcel = new List<StorageLocInfo>();

        // 略过标题行，所以 rowIndex = 1 开始
        var lastRowNum = sheet.LastRowNum;
        for (var rowIndex = 1; rowIndex <= lastRowNum; rowIndex++) {
            var row = sheet.GetRow(rowIndex);

            if (row == null) break;

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            var cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++) row.GetCell(cellIndex)?.SetCellType(CellType.String);

            // 层 列 下架顺序
            cellIndex = 0;
            var rowNumStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rowNumStr)) break;

            var colNumStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            var offShelfOrderStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            if (int.TryParse(rowNumStr, out var rowNum) == false) {
                throw new ValidationException($"第{rowIndex}行数据 行数 数据格式不对！");
            }

            if (int.TryParse(colNumStr, out var colNum) == false) {
                throw new ValidationException($"第{rowIndex}行数据 列数 数据格式不对！");
            }

            if (int.TryParse(offShelfOrderStr, out var offShelfOrderNum) == false) {
                throw new ValidationException($"第{rowIndex}行数据 下架顺序 数据格式不对！");
            }

            var storageLoc = new StorageLocInfo();
            storageLoc.RowNum = rowNum;
            storageLoc.ColNum = colNum;
            storageLoc.OffShelfOrderNum = offShelfOrderNum;

            var slInDb = storageLocListInDb.Where(e => e.RowNum == storageLoc.RowNum && e.ColNum == storageLoc.ColNum).Take(1).SingleOrDefault();
            if (slInDb == null) {
                throw new ValidationException($"第{rowIndex}行数据 指定的行{rowNum}列{colNum}仓位，在当前货架不存在！");
            }

            storageLocListFromExcel.Add(storageLoc);
        }

        foreach (var sl in storageLocListFromExcel) {
            var slInDb = storageLocListInDb.Where(e => e.RowNum == sl.RowNum && e.ColNum == sl.ColNum).Take(1).SingleOrDefault();

            if (slInDb != null) {
                slInDb.OffShelfOrderNum = sl.OffShelfOrderNum;
                session.Update(slInDb);
            }
        }

        session.Flush();
    }
}