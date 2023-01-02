using Microsoft.AspNetCore.Http;
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
public class LogisticsGoodsTradeExcelService {
    private readonly ISession session;
    private readonly LogisticsGoodsLogService logisticsGoodsLogService;

    public LogisticsGoodsTradeExcelService(ISession session,
        LogisticsGoodsLogService logisticsGoodsLogService
    ) {
        this.session = session;
        this.logisticsGoodsLogService = logisticsGoodsLogService;
    }

    public byte[] Export(IList<LogisticsGoodsInfo> logisticsGoodsList) {
        string sheetName = "确认扩展名";

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        // 扩展名    物品类型    品牌    中国海关申报名    中国海关编码
        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        int cellIndex = 0;
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("扩展名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("物品类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("品牌");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关申报名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关编码");

        foreach (LogisticsGoodsInfo goods in logisticsGoodsList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 扩展名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.Name);

            // 物品类型
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.Type.Name);

            // 品牌
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.BrandName);

            // 中国海关申报名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.HsGoods?.CnHsName ?? string.Empty);

            // 中国海关编码
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.HsGoods?.CnHsCode ?? string.Empty);
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.ToArray();
    }

    public void Import(IFormFile excelFile, UserInfo loginUser) {
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null) {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<LogisticsGoodsInfo> logisticsGoodsList = new List<LogisticsGoodsInfo>();

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
                row.GetCell(cellIndex)?.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 扩展名
            string name = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name)) {
                break;
            }

            name = name.Trim();

            // 物品类型
            string goodsTypeStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            if (string.IsNullOrWhiteSpace(goodsTypeStr)) {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 不能为空，导入失败！");
            }

            LogisticsGoodsType goodsType = LogisticsGoodsType.GetByName(goodsTypeStr.Trim());
            if (goodsType == LogisticsGoodsType.None) {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 数据有误，导入失败！");
            }

            // 品牌
            string brandName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            LogisticsGoodsInfo? logisticsGoodsInDb = session
                .Query<LogisticsGoodsInfo>()
                .Where(e => e.Name == name
                            && e.Type == goodsType
                            && e.BrandName == brandName)
                .Take(1)
                .SingleOrDefault();
            if (logisticsGoodsInDb == null) {
                throw new ValidationException($"第{rowIndex}行数据指定的 扩展名+特征属性 不存在，导入失败！");
            }

            logisticsGoodsInDb.Name = name;
            logisticsGoodsInDb.Type = goodsType;
            logisticsGoodsInDb.BrandName = brandName;

            // 中国海关申报名
            string cnHsName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cnHsName)) {
                throw new ValidationException($"第{rowIndex}行数据 中国海关申报名 不能为空，导入失败！");
            }

            cnHsName = cnHsName.Trim();

            HsGoodsInfo? hsGoods = session.Query<HsGoodsInfo>()
                .Where(e => e.CnHsName == cnHsName)
                .Take(1)
                .SingleOrDefault();

            if (hsGoods == null) {
                throw new ValidationException($"第{rowIndex}行数据指定的 中国海关申报名 不存在，导入失败！");
            }

            logisticsGoodsInDb.HsGoods = hsGoods;

            logisticsGoodsList.Add(logisticsGoodsInDb);
        }

        // 数据库事务
        Transactional.Wrap(session, () => {
            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
                logisticsGoods.UpdateUser = loginUser;
                logisticsGoods.Status = LogisticsGoodsStatus.Confirmed;
                session.Update(logisticsGoods);
            }

            session.Flush();

            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
                logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Confirm, logisticsGoods.Id, loginUser.Id);
            }

            session.Flush();
        });
    }
}