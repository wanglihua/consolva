using Microsoft.AspNetCore.Http;
using NHibernate.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.LogisticsTrack;
using shunda.Domain.Pass;
using shunda.Service.logisticsTrack.SeventeenTrack.Carrier;
using shunda.Service.logisticsTrack.SeventeenTrack.Register;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

/// <summary>
/// 物流订单头程 Excel 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsOrderFirstModeExcelService {
    private readonly ISession session;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly SeventeenTrackRegisterService seventeenTrackRegisterService;
    private readonly SeventeenTrackCarrierService seventeenTrackCarrierService;

    public LogisticsOrderFirstModeExcelService(
        ISession session,
        LogisticsOrderActionService logisticsOrderActionService,
        SeventeenTrackRegisterService seventeenTrackRegisterService,
        SeventeenTrackCarrierService seventeenTrackCarrierService
    ) {
        this.session = session;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.seventeenTrackRegisterService = seventeenTrackRegisterService;
        this.seventeenTrackCarrierService = seventeenTrackCarrierService;
    }

    /// <summary>
    /// 导出Excel用于国际单号导入
    /// </summary>
    /// <param name="orderFirstModeList"></param>
    /// <returns></returns>
    public byte[] Export(IList<LogisticsOrderFirstModeInfo> orderFirstModeList) {
        IWorkbook workbook = new XSSFWorkbook();

        string sheetName = "订单国际单号列表";
        ISheet sheet = workbook.CreateSheet(sheetName);

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
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

        sheet.SetColumnWidth(cellIndex, 20 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("数据ID(方便导入)");

        sheet.SetColumnWidth(cellIndex, 20 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("订单编号");

        sheet.SetColumnWidth(cellIndex, 100 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("物流方式");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("物品类型");

        sheet.SetColumnWidth(cellIndex, 25 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("国际单号");

        foreach (var orderFirstMode in orderFirstModeList) {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 数据ID
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue($"{orderFirstMode.Id}#{orderFirstMode.Order.Id}#{orderFirstMode.FirstMode.Id}");

            // 订单编号
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderFirstMode.OrderCode);

            // 物流方式
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderFirstMode.FirstMode.Name);

            // 物品类型
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderFirstMode.GoodsType.Name);

            // 国际单号
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderFirstMode.TrackingNo);
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return memoryStream.GetBuffer();
    }

    public void Import(IFormFile excelFile, int loginUserId) {
        if (excelFile == null) {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null) {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        //  LogisticsOrderFirstModeExcelImportItem

        IList<LogisticsOrderFirstModeExcelImportItem> importItemList = new List<LogisticsOrderFirstModeExcelImportItem>();

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

            string dataIdStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dataIdStr)) {
                break;
            }

            string[] dataIdArray = dataIdStr.Split('#', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (dataIdArray == null || dataIdArray.Length != 3) {
                throw new ValidationException($"第{rowIndex}行数据 数据ID格式不正确#0，导入失败！");
            }

            if (int.TryParse(dataIdArray[0], out int orderFirstModeId) == false) {
                throw new ValidationException($"第{rowIndex}行数据 数据ID格式不正确#1，导入失败！");
            }

            if (int.TryParse(dataIdArray[1], out int orderId) == false) {
                throw new ValidationException($"第{rowIndex}行数据 数据ID格式不正确#2，导入失败！");
            }

            if (int.TryParse(dataIdArray[2], out int firstModeId) == false) {
                throw new ValidationException($"第{rowIndex}行数据 数据ID格式不正确#3，导入失败！");
            }

            string orderCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(orderCode) == true) {
                throw new ValidationException($"第{rowIndex}行数据 订单编号不能为空，导入失败！");
            }

            // 略过物流方式
            cellIndex++;

            // 略过物品类型
            cellIndex++;

            string trackingNo = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trackingNo) == true) {
                throw new ValidationException($"第{rowIndex}行数据 国际单号不能为空，导入失败！");
            }

            importItemList.Add(new LogisticsOrderFirstModeExcelImportItem {
                OrderFirstModeId = orderFirstModeId,
                OrderId = orderId,
                FirstModeId = firstModeId,
                OrderCode = orderCode.Trim(),
                TrackingNo = trackingNo.Trim(),
            });
        }

        IList<int> orderFirstModeIdList = importItemList.Select(e => e.OrderFirstModeId).ToList();
        // IList<int> orderIdList = importItemList.Select(e => e.OrderId).Distinct().ToList();
        // IList<int> firstModeIdList = importItemList.Select(e => e.FirstModeId).Distinct().ToList();

        var orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Where(e => orderFirstModeIdList.Contains(e.Id))
            .ToList();

        int importItemCount = importItemList.Count;
        for (int index = 0; index < importItemCount; index++) {
            var importItem = importItemList[index];

            int orderFirstModeId = importItem.OrderFirstModeId;
            int orderId = importItem.OrderId;
            int firstModeId = importItem.FirstModeId;
            string orderCode = importItem.OrderCode;

            // 判断数据ID是否存在
            var orderFirstMode = orderFirstModeList.Where(e => e.Id == orderFirstModeId).Take(1).SingleOrDefault();
            if (orderFirstMode == null) {
                throw new ValidationException($"第{index + 1}行数据 数据ID不存在#1，导入失败！");
            }

            // 判断数据ID之间，以及订单编号等是否匹配
            if (orderFirstMode.Order.Id != orderId) {
                throw new ValidationException($"第{index + 1}行数据 数据ID不存在#2，导入失败！");
            }

            if (orderFirstMode.FirstMode.Id != firstModeId) {
                throw new ValidationException($"第{index + 1}行数据 数据ID不存在#3，导入失败！");
            }

            if (orderFirstMode.OrderCode != orderCode) {
                throw new ValidationException($"第{index + 1}行数据 订单编号和数据ID不匹配，导入失败！");
            }

            if (string.IsNullOrWhiteSpace(importItem.TrackingNo)) {
                throw new ValidationException($"第{index + 1}行数据 国际单号不能为空，导入失败！");
            }

            importItem.OrderFirstMode = orderFirstMode;
        }

        IList<LogisticsOrderFirstModeExcelImportItem> actualImportItemList = new List<LogisticsOrderFirstModeExcelImportItem>();

        foreach (var importItem in importItemList) {
            var orderFirstMode = importItem.OrderFirstMode;

            if (orderFirstMode.TrackingNo != importItem.TrackingNo) {
                orderFirstMode.TrackingNo = importItem.TrackingNo;
                session.Update(orderFirstMode);

                actualImportItemList.Add(importItem);
            }
        }

        session.Flush();

        // 订单日志
        foreach (var importItem in actualImportItemList) {
            logisticsOrderActionService.NewActionUser(importItem.OrderId, LogisticsOrderActionType.OrderFirstModeImportTrackingNo, importItem.TrackingNo, loginUserId);
        }

        session.Flush();

        // 如果已经存在了就不用注册了
        var trackingNoList = actualImportItemList.Select(e => e.TrackingNo).ToList();
        var trackNumberList = session.Query<SeventeenTrackNumberInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo)
                        && (e.Status == SeventeenTrackNumberStatus.Accepted || e.Status == SeventeenTrackNumberStatus.Completed))
            .ToList();
        var trackNumberDict = trackNumberList.ToDictionary(e => e.TrackingNo, e => e);

        var trackingNoListToRegister = new List<string>();
        foreach (string trackingNo in trackingNoList) {
            if (trackNumberDict.ContainsKey(trackingNo) == false) {
                trackingNoListToRegister.Add(trackingNo);
            }
        }

        trackingNoListToRegister = trackingNoListToRegister.Distinct().ToList(); // 防止重复

        // 注册单号
        if (trackingNoListToRegister.Count != 0) {
            seventeenTrackRegisterService.Register(
                trackingNoListToRegister
                    .Select(e => new SeventeenTrackRegisterRequestItem() {
                        Number = e,
                        Carrier = seventeenTrackCarrierService.GetKeyByTrackingNo(e),
                    })
                    .ToList()
            );
        }
    }
}