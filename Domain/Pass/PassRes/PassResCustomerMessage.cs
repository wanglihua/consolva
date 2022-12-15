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
            new PassResInfo() { Id = MessageSetting, Name = "消息设置", CateIdForCache = CateId },
            new PassResInfo() { Id = MessageView, Name = "消息查看", CateIdForCache = CateId },
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
        /// 消息设置
        /// </summary>
        public const int MessageSetting = CateId + 06000;

        /// <summary>
        /// 消息查看
        /// </summary>
        public const int MessageView = CateId + 8000;
    }
}