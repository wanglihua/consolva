using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;

namespace shunda.Service.Pass;

[Service(ServiceType.Scoped)]
public class PassResCateDataInitService
{
    private ISession session;

    public PassResCateDataInitService(ISession session)
    {
        this.session = session;
    }

    public void InitData()
    {
        IList<PassResCateInfo> cateList = new List<PassResCateInfo>() {
            PassRes.OrderFee.Cate,
            PassRes.Storage.Cate,
            PassRes.Logistics.Cate,
            PassRes.CustomerMessage.Cate,
        };

        // 检查 cate id 唯一性
        Dictionary<int, PassResCateInfo> cateIdMap = new Dictionary<int, PassResCateInfo>();
        foreach (PassResCateInfo cate in cateList)
        {
            if (cateIdMap.TryGetValue(cate.Id, out PassResCateInfo? cateExist))
            {
                throw new Exception($"授权资源类别 {cate.Name} 和 {cateExist.Name} 两者 id#{cate.Id} 相重！");
            }

            cateIdMap[cate.Id] = cate;
        }

        IList<int> cateIdList = cateList.Select(e => e.Id).ToList();
        IList<PassResCateInfo> cateListInDb = session.Query<PassResCateInfo>()
            .Where(e => cateIdList.Contains(e.Id))
            .ToList();

        foreach (PassResCateInfo cate in cateList)
        {
            int cateId = cate.Id;
            PassResCateInfo? cateInDb = cateListInDb
                .Where(e => e.Id == cateId)
                .Take(1)
                .SingleOrDefault();

            bool shouldSave = false;
            if (cateInDb == null)
            {
                shouldSave = true;
                cateInDb = new PassResCateInfo();
                cateInDb.Id = cate.Id;
            }

            if (cateInDb.Name != cate.Name)
            {
                shouldSave = true;
                cateInDb.Name = cate.Name;
            }

            if (cateInDb.Description != cate.Description)
            {
                shouldSave = true;
                cateInDb.Description = cate.Description;
            }

            if (shouldSave)
            {
                session.SaveOrUpdate(cateInDb);
            }
        }

        session.Flush();
    }
}