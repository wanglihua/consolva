namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 快递管理
    /// </summary>
    public class Express {
        public const int CateId = PassResCate.ExpressCateId;
        public static readonly PassResCateInfo Cate = PassResCate.Express;

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = ExpressList, Name = "快递列表", Cate = Cate },
            new PassResInfo { Id = ExpressRemarkEdit, Name = "快递单备注维护", Cate = Cate },
            new PassResInfo { Id = ExpressForceDelete, Name = "快递强制删除", Cate = Cate },
            new PassResInfo { Id = ExpressThrowAway, Name = "快递单丢弃", Cate = Cate },
            new PassResInfo { Id = ExpressExpect, Name = "预报单号", Cate = Cate },
            new PassResInfo { Id = ExpressPhoto, Name = "快递单图片", Cate = Cate },
            new PassResInfo { Id = ExpressPhotoDelete, Name = "快递单图片删除", Cate = Cate },
            new PassResInfo { Id = ExpressReturn, Name = "退运池", Cate = Cate },
            new PassResInfo { Id = ExpressSizeWeight, Name = "快递单尺寸重量", Cate = Cate },
            new PassResInfo { Id = ExpressRemarkPresetSelect, Name = "快递单备注预设", Cate = Cate },
            new PassResInfo { Id = ExpressListToOffShelf, Name = "待下架快递", Cate = Cate },
            new PassResInfo { Id = ExpressOffShelfSchedule, Name = "快递下架排班", Cate = Cate },
        };

        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 快递列表
        /// </summary>
        public const int ExpressList = CateId + 0200;

        /// <summary>
        /// 快递单备注维护
        /// 包括增加修改删除
        /// </summary>
        public const int ExpressRemarkEdit = CateId + 0400;

        /// <summary>
        /// 快递强制删除
        /// </summary>
        public const int ExpressForceDelete = CateId + 0600;

        /// <summary>
        /// 快递单丢弃
        /// </summary>
        public const int ExpressThrowAway = CateId + 0800;

        /// <summary>
        /// 预报单号
        /// </summary>
        public const int ExpressExpect = CateId + 1000;

        /// <summary>
        /// 快递单图片
        /// </summary>
        public const int ExpressPhoto = CateId + 1200;

        /// <summary>
        /// 快递单图片删除
        /// </summary>
        public const int ExpressPhotoDelete = CateId + 1400;

        /// <summary>
        /// 退运池
        /// </summary>
        public const int ExpressReturn = CateId + 1600;

        /// <summary>
        /// 快递单尺寸重量
        /// </summary>
        public const int ExpressSizeWeight = CateId + 1800;

        /// <summary>
        /// 快递单备注预设
        /// </summary>
        public const int ExpressRemarkPresetSelect = CateId + 2000;

        /// <summary>
        /// 待下架快递
        /// </summary>
        public const int ExpressListToOffShelf = CateId + 2200;

        /// <summary>
        /// 快递下架排班
        /// </summary>
        public const int ExpressOffShelfSchedule = CateId + 2400;
    }
}
