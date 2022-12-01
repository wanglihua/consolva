using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using NHibernate;
using NLog;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;

namespace shunda.Service;

[Service(ServiceType.Scoped)]
public class SiteRequestLogService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private static readonly int maxUrlLength = 500 - 10;
    private static readonly int maxIpAddressLength = 200 - 5;

    private ISessionFactory sessionFactory;
    private WebSessionService webSessionService;

    public SiteRequestLogService(ISessionFactory sessionFactory,
        WebSessionService webSessionService)
    {
        this.sessionFactory = sessionFactory;
        this.webSessionService = webSessionService;
    }

    public void NewSiteRequest(HttpRequest request, int userId, decimal totalSeconds, bool useInnerTransaction)
    {
        if (SiteConfig.RequestLogEnabled == false)
        {
            return;
        }

#if !DEBUG
        if (userId == shunda.Domain.Pass.UserInfo.DevUserId)
        {
            return;
        }

        if (userId != shunda.Domain.Pass.UserInfo.AdminUserId) // 只记录管理员的
        {
            return;
        }
#endif

        IStatelessSession? statelessSession = null;
        ITransaction? transaction = null;

        try
        {
            statelessSession = sessionFactory.OpenStatelessSession();

            if (useInnerTransaction)
            {
                transaction = statelessSession.BeginTransaction();
            }

            string sessionId = webSessionService.SessionId;

            // ipAddress
            string ipAddress = WebUtil.GetClientIp(request);
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = string.Empty;
            }

            if (ipAddress.Length > maxIpAddressLength)
            {
                ipAddress = ipAddress.Substring(0, maxUrlLength);
            }

            // url
            string url = request.GetEncodedUrl();
            if (string.IsNullOrWhiteSpace(url))
            {
                url = string.Empty;
            }

            if (url.Length > maxUrlLength)
            {
                url = url.Substring(0, maxUrlLength);
            }

            SiteRequestLogInfo siteRequestLog = new SiteRequestLogInfo();
            siteRequestLog.SessionId = sessionId;
            siteRequestLog.IpAddress = ipAddress;
            if (userId != 0)
            {
                siteRequestLog.UserId = userId;
            }
            else
            {
                siteRequestLog.UserId = null;
            }

            siteRequestLog.CustomerId = null;

            siteRequestLog.Url = url;
            siteRequestLog.TotalSeconds = totalSeconds;
            siteRequestLog.CreateTime = DateTime.Now;

            if (request.Method?.ToUpper() == "POST" && request.ContentType?.Contains("application/x-www-form-urlencoded") == true)
            {
                ICollection<string> formKeyList = request.Form.Keys;
                if (formKeyList != null && formKeyList.Count != 0)
                {
                    IList<string> formKeyValueList = new List<string>();
                    foreach (string formKey in formKeyList)
                    {
                        if (formKey != "Password" && request.Form.TryGetValue(formKey, out StringValues formValues))
                        {
                            string formValuesStr = string.Join(",", formValues);
                            if (string.IsNullOrWhiteSpace(formValuesStr) == false && formValuesStr != "0")
                            {
                                formKeyValueList.Add($"{formKey}:{formValuesStr}");
                            }
                        }
                    }

                    string formKeyValueStrs = string.Join(";", formKeyValueList);
                    if (formKeyValueStrs.Length > 500)
                    {
                        formKeyValueStrs = formKeyValueStrs.Substring(0, 500);
                    }

                    siteRequestLog.FormParams = formKeyValueStrs;
                }
            }

            statelessSession.Insert(siteRequestLog);

            if (useInnerTransaction)
            {
                transaction?.Commit();
            }
        }
        catch (Exception exp)
        {
            if (useInnerTransaction)
            {
                transaction?.Rollback();
            }

            log.Error(exp);
        }
        finally
        {
            statelessSession?.Close();
        }
    }
}