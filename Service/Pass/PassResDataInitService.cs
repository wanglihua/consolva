using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;

namespace shunda.Service.Pass;

[Service(ServiceType.Scoped)]
public class PassResDataInitService {
    private readonly ISession session;

    public PassResDataInitService(ISession session) {
        this.session = session;
    }

    public void InitData() {
        List<PassResInfo> passResList = new List<PassResInfo>();
        passResList.AddRange(PassRes.LogisticsOrder.AllData);
        passResList.AddRange(PassRes.Express.AllData);
        passResList.AddRange(PassRes.LogisticsTrack.AllData);
        passResList.AddRange(PassRes.ReceivePack.AllData);
        passResList.AddRange(PassRes.Fee.AllData);
        passResList.AddRange(PassRes.Customer.AllData);
        passResList.AddRange(PassRes.Storage.AllData);
        passResList.AddRange(PassRes.Logistics.AllData);
        passResList.AddRange(PassRes.Goods.AllData);
        passResList.AddRange(PassRes.CustomerMessage.AllData);
        passResList.AddRange(PassRes.Setting.AllData);
        passResList.AddRange(PassRes.AdminApp.AllData);

        // 检查 pass res id 唯一性
        Dictionary<int, PassResInfo> passResIdMap = new Dictionary<int, PassResInfo>();
        foreach (PassResInfo passRes in passResList) {
            if (passResIdMap.TryGetValue(passRes.Id, out PassResInfo? passResExist)) {
                throw new Exception($"授权资源 {passRes.Name} 和 {passResExist.Name} 两者 id#{passRes.Id} 相重！");
            }

            passResIdMap[passRes.Id] = passRes;
        }

        IList<int> passResIdList = passResList.Select(e => e.Id).ToList();
        IList<PassResInfo> passResListInDb = session.Query<PassResInfo>()
            .Where(e => passResIdList.Contains(e.Id))
            .ToList();

        foreach (PassResInfo passRes in passResList) {
            int passResId = passRes.Id;
            PassResInfo? passResInDb = passResListInDb
                .Where(e => e.Id == passResId)
                .Take(1)
                .SingleOrDefault();

            bool shouldSave = false;
            bool isUpdate = true;
            if (passResInDb == null) {
                shouldSave = true;
                isUpdate = false;
                passResInDb = new PassResInfo {
                    Id = passRes.Id
                };
            }

            if (passResInDb.Cate.Id != passRes.Cate.Id) {
                shouldSave = true;
                passResInDb.Cate = session.Load<PassResCateInfo>(passRes.Cate.Id);
            }

            if (passResInDb.Name != passRes.Name) {
                shouldSave = true;
                passResInDb.Name = passRes.Name;
            }

            if (passResInDb.Description != passRes.Description) {
                shouldSave = true;
                passResInDb.Description = passRes.Description;
            }

            if (shouldSave) {
                if (isUpdate) {
                    session.Update(passResInDb);
                } else {
                    session.Save(passResInDb);
                }
            }
        }

        session.Flush();
    }
}