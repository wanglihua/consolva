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
            new PassResInfo() { Id = Warehouse, Name = "仓库", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 仓库
        /// </summary>
        public const int Warehouse = CateId + 01000;
        
    }
}