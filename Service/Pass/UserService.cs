using NHibernate;
using shunda.Core.Service;

namespace shunda.Service.Pass;

/// <summary>
/// 用户 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class UserService {
    private readonly ISession session;
    private readonly AdminUserService adminUserService;

    public UserService(ISession session, AdminUserService adminUserService) {
        this.session = session;
        this.adminUserService = adminUserService;
    }

    public bool IsUserAdmin(int userId) {
        return adminUserService.IsUserAdmin(userId);
    }
}