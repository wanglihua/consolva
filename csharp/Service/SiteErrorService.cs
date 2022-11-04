using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain;

namespace shunda.Service;

[Service(ServiceType.Scoped)]
public class SiteErrorService
{
    private ISession session;

    public SiteErrorService(ISession session)
    {
        this.session = session;
    }

    public void AddSiteError(SiteErrorType type, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            return;
        }

        if (errorMessage.Length > 500)
        {
            errorMessage = errorMessage.Substring(0, 500);
        }

        string subType = Md5Util.Encrypt(errorMessage);

        // 如果已存在同类型的未解决的错误，就不用插入新的错误信息
        var siteErrorSameType = session.Query<SiteErrorInfo>()
            .Where(e => e.Type == type && e.SubType == subType && e.Resolved == false)
            .Take(1)
            .SingleOrDefault();
        if (siteErrorSameType != null)
        {
            return;
        }

        var siteError = new SiteErrorInfo();

        siteError.Type = type;
        siteError.SubType = subType;
        siteError.ErrorMsg = errorMessage;
        siteError.Resolved = false;

        session.Save(siteError);
        session.Flush();
    }

    public IList<SiteErrorInfo> GetNotResolvedErrorList()
    {
        var errorList = session.Query<SiteErrorInfo>()
            .Where(e => e.Resolved == false)
            .OrderByDescending(e => e.CreateTime)
            .ToList();

        return errorList;
    }

    public void SetResolved(int siteErrorId)
    {
        var siteError = session.Get<SiteErrorInfo>(siteErrorId);
        if (siteError == null)
        {
            throw new ValidationException("指定的网站错误不存在！");
        }

        if (siteError.Resolved == true)
        {
            throw new ValidationException("指定的网站错误已经是已解决状态！");
        }

        siteError.Resolved = true;

        session.Update(siteError);
        session.Flush();
    }
}