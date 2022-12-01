namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 消息推送
    /// </summary>
    public class CustomerMessage
    {
        /// <summary>
        /// 消息推送
        /// </summary>
        public const int CateId = 11000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "消息推送" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = ManualSend, Name = "手动推送", CateIdForCache = CateId },
            new PassResInfo() { Id = ManualSendAll, Name = "全员推送", CateIdForCache = CateId },
            new PassResInfo() { Id = CycleTiming, Name = "定时任务推送", CateIdForCache = CateId },
            new PassResInfo() { Id = RealTime, Name = "实时任务提醒", CateIdForCache = CateId },
            new PassResInfo() { Id = MessageList, Name = "消息列表", CateIdForCache = CateId },
            new PassResInfo() { Id = SmsSendRecord, Name = "短信发送记录", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 手动推送
        /// </summary>
        public const int ManualSend = CateId + 02000;

        /// <summary>
        /// 全员推送
        /// </summary>
        public const int ManualSendAll = CateId + 04000;

        /// <summary>
        /// 定时任务推送
        /// </summary>
        public const int CycleTiming = CateId + 06000;

        /// <summary>
        /// 实时任务提醒
        /// </summary>
        public const int RealTime = CateId + 08000;

        /// <summary>
        /// 消息列表
        /// </summary>
        public const int MessageList = CateId + 10000;

        /// <summary>
        /// 短信发送记录
        /// </summary>
        public const int SmsSendRecord = CateId + 12000;
    }
}