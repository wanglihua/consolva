using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Job.Middlewares;

/// <summary>
/// 当前只是用来关闭 session
/// 应该是配置成最后一个执行的middleware
/// </summary>
public class HibernateMiddleware
{
    private readonly RequestDelegate next;

    public HibernateMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, [FromServices] ISession session)
    {
        await next.Invoke(context);

        session.Close();
    }
}