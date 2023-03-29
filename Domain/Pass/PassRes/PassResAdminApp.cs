namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 仓管APP
    /// </summary>
    public class PassResAdminApp {
        public const int CateId = PassResCate.AdminAppCateId;
        public static readonly PassResCateInfo Cate = PassResCate.AdminApp;


        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        ///  快递单号查询
        /// </summary>
        public const int ExpressQuery = CateId + 0200;

        /// <summary>
        /// 快递上架
        /// </summary>
        public const int ExpressOnShelf = CateId + 0200;

        /// <summary>
        /// 仓库需核实
        /// </summary>
        public const int ExpressWarehouseCheck = CateId + 0400;

        /// <summary>
        /// 快递下架
        /// </summary>
        public const int ExpressOffShelf = CateId + 0600;

        /// <summary>
        /// 退运下架
        /// </summary>
        public const int ExpressReturnOffShelf = CateId + 0800;

        /// <summary>
        /// 快递堆存
        /// </summary>
        public const int ExpressDump = CateId + 1000;

        /// <summary>
        /// 入流动仓
        /// </summary>
        public const int ExpressPacking = CateId + 1200;

        /// <summary>
        /// 箱号信息查询
        /// </summary>
        public const int PackBoxQuery = CateId + 1000;

        /// <summary>
        /// 箱号发货
        /// </summary>
        public const int PackBoxShip = CateId + 1400;

        /// <summary>
        /// 图片上传
        /// </summary>
        public const int ExpressPhoto = CateId + 1000;
    }
}



/*
  a

*/