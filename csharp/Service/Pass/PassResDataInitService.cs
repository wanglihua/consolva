using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;

namespace shunda.Service.Pass;

[Service(ServiceType.Scoped)]
public class PassResDataInitService
{
    private ISession session;

    public PassResDataInitService(ISession session)
    {
        this.session = session;
    }

    public void InitData()
    {
        List<PassResInfo> passResList = new List<PassResInfo>();
        passResList.AddRange(PassRes.Storage.AllData);

        IList<int> passResIdList = passResList.Select(e => e.Id).ToList();
        IList<PassResInfo> passResListInDb = session.Query<PassResInfo>()
            .Where(e => passResIdList.Contains(e.Id))
            .ToList();

        foreach (PassResInfo passRes in passResList)
        {
            int passResId = passRes.Id;
            PassResInfo? passResInDb = passResListInDb
                .Where(e => e.Id == passResId)
                .Take(1)
                .SingleOrDefault();

            bool shouldSave = false;
            if (passResInDb == null)
            {
                shouldSave = true;
                passResInDb = new PassResInfo();
                passResInDb.Id = passRes.Id;
            }

            if (passResInDb.Cate.Id != passRes.CateIdForCache)
            {
                shouldSave = true;
                passResInDb.Cate = session.Load<PassResCateInfo>(passRes.CateIdForCache);
            }

            if (passResInDb.Name != passRes.Name)
            {
                shouldSave = true;
                passResInDb.Name = passRes.Name;
            }

            if (passResInDb.Description != passRes.Description)
            {
                shouldSave = true;
                passResInDb.Description = passRes.Description;
            }

            if (shouldSave)
            {
                session.SaveOrUpdate(passResInDb);
            }
        }

        session.Flush();
    }
}