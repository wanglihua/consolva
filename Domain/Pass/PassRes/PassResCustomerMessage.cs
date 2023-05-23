namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 消息推送
    /// </summary>
    public class CustomerMessage {
        public const int CateId = PassResCate.CustomerMessageCateId;
        public static PassResCateInfo Cate => PassResCate.CustomerMessage;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = ManualSend, Name = "手动推送", Cate = Cate },
            new PassResInfo { Id = ManualSendAll, Name = "全员推送", Cate = Cate },
            new PassResInfo { Id = MessageSetting, Name = "消息设置", Cate = Cate },
            new PassResInfo { Id = MessageView, Name = "消息查看", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 手动推送
        /// </summary>
        public const int ManualSend = CateId + 0200;

        /// <summary>
        /// 全员推送
        /// </summary>
        public const int ManualSendAll = CateId + 0400;

        /// <summary>
        /// 消息设置
        /// </summary>
        public const int MessageSetting = CateId + 0600;

        /// <summary>
        /// 消息查看
        /// </summary>
        public const int MessageView = CateId + 0800;
    }
}