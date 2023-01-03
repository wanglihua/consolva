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
public class LogisticsGoodsWarehouseExcelService {
    private readonly ISession session;

    public LogisticsGoodsWarehouseExcelService(ISession session) {
        this.session = session;
    }

    public byte[] Export(IList<LogisticsGoodsInfo> logisticsGoodsList) {
        string sheetName = "扩展名添加";

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        // 扩展名    物品类型    品牌
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

        /*
        sheet.SetColumnWidth(cellIndex, 10 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("状态");
        */

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

            /*
            // 状态
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(LogisticsGoodsStatus.GetById(goods.StatusId)?.Name);
            */
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

            LogisticsGoodsInfo logisticsGoods = new LogisticsGoodsInfo();

            cellIndex = 0;

            // 物品类型
            string goodsTypeStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(goodsTypeStr)) {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 不能为空，导入失败！");
            }

            LogisticsGoodsType goodsType = LogisticsGoodsType.GetByName(goodsTypeStr.Trim());
            if (goodsType == LogisticsGoodsType.None) {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 数据有误，导入失败！");
            }

            logisticsGoods.Type = goodsType;

            // 扩展名
            string name = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name)) {
                break;
            }

            name = name.Trim();

            if (name.Length > 200) {
                throw new ValidationException($"第{rowIndex}行数据 扩展名 {name} 太长，超过200长度，导入失败！");
            }

            logisticsGoods.Name = name.Trim();

            // 品牌
            string brandName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            logisticsGoods.BrandName = brandName;

            logisticsGoodsList.Add(logisticsGoods);
        }

        // 扩展名重复检查

        // 自身导入的判断重复
        IDictionary<string, int> logisticsGoodsNameRowNumMap = new Dictionary<string, int>();
        int logisticsGoodsRowNum = 1;
        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) // 这里logisticsGoodsList还是保持了导入顺序
        {
            string name = logisticsGoods.Name;
            if (logisticsGoodsNameRowNumMap.TryGetValue(name, out int rowNumExist) == true) {
                throw new ValidationException($"导入数据中，第{logisticsGoodsRowNum}行 扩展名 {name} 跟第{rowNumExist}行 数据重复！");
            }

            logisticsGoodsNameRowNumMap[name] = logisticsGoodsRowNum;
            logisticsGoodsRowNum = logisticsGoodsRowNum + 1;
        }

        // 跟数据中现有的判断重复
        IList<string> goodsNameList = logisticsGoodsList.Select(e => e.Name).ToList();
        IList<LogisticsGoodsInfo> logisticsGoodsListExist = session.Query<LogisticsGoodsInfo>()
            .Where(e => goodsNameList.Contains(e.Name))
            .ToList();
        IDictionary<string, int> logisticsGoodsMapExist = logisticsGoodsListExist.ToDictionary(e => e.Name, e => e.Id);

        logisticsGoodsRowNum = 1;
        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) // 这里logisticsGoodsList还是保持了导入顺序
        {
            string name = logisticsGoods.Name;
            if (logisticsGoodsMapExist.ContainsKey(name)) {
                throw new ValidationException($"导入数据中，第{logisticsGoodsRowNum}行 扩展名 {name} 数据库中已存在！");
            }

            logisticsGoodsRowNum = logisticsGoodsRowNum + 1;
        }

        Transactional.Wrap(session, () => {
            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) {
                // 海关编码暂时置空
                logisticsGoods.HsGoods = null;
                logisticsGoods.AddUser = loginUser;
                logisticsGoods.UpdateUser = loginUser;
                logisticsGoods.Status = LogisticsGoodsStatus.NotConfirmed;

                session.Save(logisticsGoods);
            }

            session.Flush();
        });
    }
}