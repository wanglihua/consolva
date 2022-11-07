using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Hibernate;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Filters;

/// <summary>
///     "services.AddScoped&lt;HibernateActionFilter&gt;();" // 必须是 AddScoped
/// </summary>
public class HibernateActionFilter : IActionFilter
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private bool autoTransaction;
    private ITransaction? transaction;

    public HibernateActionFilter(ISession session)
    {
        this.session = session;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // log.Trace("HibernateActionFilter OnActionExecuting");

        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor == null) return;

        var controllerAttributes = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(TransactionalAttribute), true);
        var actionAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(TransactionalAttribute), true);

        autoTransaction = false;
        if ((controllerAttributes?.Length ?? 0) != 0) autoTransaction = true;

        if ((actionAttributes?.Length ?? 0) != 0) autoTransaction = true;

        if (autoTransaction) transaction = session.BeginTransaction();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // log.Trace("HibernateActionFilter OnActionExecuted");

        // 这里是捕捉到非数据库异常的情况

        if (context.Exception != null)
        {
            if (context.Exception is ValidationException == false && context.Exception is System.ComponentModel.DataAnnotations.ValidationException == false)
                // 这里不记录异常也是可以的，因为异常统一处理程序会记录
                log.Error(context.Exception);

            if (autoTransaction) transaction?.Rollback();

            // 异常会继续走向异常处理器s

            return;
        }

        // 这里捕捉不到非数据库的异常
        try
        {
            session.Flush();

            if (autoTransaction) transaction?.Commit();
        }
        catch (Exception exp)
        {
            if (context.Exception is ValidationException == false || context.Exception is System.ComponentModel.DataAnnotations.ValidationException == false) log.Error(exp);

            if (autoTransaction) transaction?.Rollback();

            throw; // 重新抛出给统一异常处理器
        }

        // session.Close(); 在这里关闭会出错
    }
}