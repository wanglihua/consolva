using NHibernate;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Storage;

namespace shunda.Service.Storage;

[Service(ServiceType.Scoped)]
public class StorageLocLabelService {
    private static readonly char Separator = '@';

    private readonly ISession session;

    public StorageLocLabelService(ISession session) {
        this.session = session;
    }

    public string GetLabelCode(int storageLocId) {
        StorageLocInfo? storageLoc = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Where(e => e.Id == storageLocId)
            .Take(1)
            .SingleOrDefault();

        if (storageLoc == null) {
            throw new ValidationException($"指定的仓位#{storageLocId}不存在！");
        }

        return GetLabelCode(storageLoc);
    }

    public string GetLabelCode(StorageLocInfo storageLoc) {
        if (storageLoc == null) {
            throw new ValidationException("指定的仓位为空！");
        }

        return $"{storageLoc.Id}{Separator}{storageLoc.Shelf?.Id ?? 0}";
    }

    public StorageLocInfo ParseLabelCode(string labelCode) {
        if (string.IsNullOrWhiteSpace(labelCode)) {
            throw new ValidationException("仓位标签编号不能为空！");
        }

        labelCode = labelCode.Trim();

        string[] labelCodeSplits = labelCode.Split(Separator);
        if (labelCodeSplits == null || labelCodeSplits.Length != 2) {
            throw new ValidationException($"仓位标签编号{labelCode}不是正确的仓位标签编号！");
        }

        if (int.TryParse(labelCodeSplits[0], out int storageLocId) == false) {
            throw new ValidationException($"仓位标签编号{labelCode}不是正确的仓位标签编号！");
        }

        if (int.TryParse(labelCodeSplits[1], out int shelfId) == false) {
            throw new ValidationException($"仓位标签编号{labelCode}不是正确的仓位标签编号！");
        }

        StorageLocInfo? storageLoc = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Where(e => e.Id == storageLocId)
            .Take(1)
            .SingleOrDefault();

        if (storageLoc == null) {
            throw new ValidationException($"仓位标签编号错误 或 仓位不存在！");
        }

        int shelfIdFromDb = storageLoc.Shelf?.Id ?? 0;
        if (shelfId != shelfIdFromDb) {
            throw new ValidationException($"仓位标签编号{labelCode}不是正确的仓位标签编号！");
        }

        return storageLoc;
    }
}