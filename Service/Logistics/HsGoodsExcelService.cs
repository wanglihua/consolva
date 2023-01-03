using Microsoft.AspNetCore.Http;
using NHibernate;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class HsGoodsExcelService {
    private readonly ISession session;

    public HsGoodsExcelService(ISession session) {
        this.session = session;
    }

    public byte[] Export(IList<HsGoodsInfo> hsGoodsList) {
        string sheetName = "海关品名";

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 中国海关申报名    中国海关编码    英文海关申报名    英文海关编码    中国申报要素1    中国申报要素2    中国申报要素3    中国申报要素4    中国申报要素5    中国申报要素6    中国申报要素7    中国申报要素8
        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        int cellIndex = 0;

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关申报名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关编码");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("英文海关申报名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("英文海关编码");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素1");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素2");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素3");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素4");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素5");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素6");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素7");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素8");

        foreach (HsGoodsInfo hsGoods in hsGoodsList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 中国海关申报名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.CnHsName);

            // 中国海关编码
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.CnHsCode);

            // 英文海关申报名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.EnHsName);

            // 英文海关编码
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.EnHsCode);

            // 中国申报要素1
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem01);

            // 中国申报要素2
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem02);

            // 中国申报要素3
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem03);

            // 中国申报要素4
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem04);

            // 中国申报要素5
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem05);

            // 中国申报要素6
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem06);

            // 中国申报要素7
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem07);

            // 中国申报要素8
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem08);
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.GetBuffer();
    }

    public void Import(IFormFile excelFile, UserInfo loginUser) {
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IList<string> cnHsNameList = session
            .Query<HsGoodsInfo>()
            .Select(e => e.CnHsName)
            .Distinct()
            .ToList();

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null) {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<HsGoodsInfo> hsGoodsList = new List<HsGoodsInfo>();

        // 略过标题行，所以 rowIndex = 1 开始
        int lastRowNum = sheet.LastRowNum;
        for (int rowIndex = 1; rowIndex <= lastRowNum; rowIndex++) {
            IRow row = sheet.GetRow(rowIndex);

            if (row == null) {
                break;
            }

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            int cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++) {
                ICell cell = row.GetCell(cellIndex);
                if (cell == null) {
                    cell = row.CreateCell(cellIndex);
                }

                cell.SetCellType(CellType.String);
            }

            cellIndex = 0;


            string cnHsName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cnHsName)) {
                break;
            }

            if (cnHsNameList.Contains(cnHsName)) {
                throw new ValidationException($"第{rowIndex}行数据 中国海关申报名 已存在或重复，导入失败！");
            }

            cnHsNameList.Add(cnHsName);

            string cnHsCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cnHsCode)) {
                throw new ValidationException($"第{rowIndex}行数据 中国海关编码 不能为空，导入失败！");
            }

            string enHsName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(enHsName)) {
                throw new ValidationException($"第{rowIndex}行数据 英文海关申报名 不能为空，导入失败！");
            }

            string enHsCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(enHsCode)) {
                throw new ValidationException($"第{rowIndex}行数据 英文海关编码 不能为空，导入失败！");
            }

            HsGoodsInfo hsGoods = new HsGoodsInfo();
            hsGoods.CnHsName = cnHsName;
            hsGoods.CnHsCode = cnHsCode;
            hsGoods.EnHsName = enHsName;
            hsGoods.EnHsCode = enHsCode;
            hsGoods.Elem01 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem02 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem03 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem04 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem05 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem06 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem07 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem08 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.AddUser = loginUser;

            hsGoodsList.Add(hsGoods);
        }

        Transactional.Wrap(session, () => {
            foreach (HsGoodsInfo hsGoods in hsGoodsList) {
                session.Save(hsGoods);
            }

            session.Flush();
        });
    }
}