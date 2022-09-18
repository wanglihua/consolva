using shunda.Core.Service;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

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
        IList<PassResCateInfo> cateList = new List<PassResCateInfo>();
        // cateList.Add(new PassResCateInfo { System = PassResSystem.Shop, Id = PassRes.ShopOrder.CateId, Name = "商城订单", Description = null });

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