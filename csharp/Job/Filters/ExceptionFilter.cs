using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NHibernate;
using NLog;
using shunda.Admin.Models.Error;
using shunda.Core;
using shunda.Core.Util;

namespace shunda.Job.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private static Logger log = LogManager.GetCurrentClassLogger();


    public ExceptionFilter()
    {
    }

    public void OnException(ExceptionContext context)
    {
        log.Trace("ExceptionFilter OnException");

        Exception exp = context.Exception ?? new Exception();

        if (exp is ValidationException == false)
        {
            log.Error(exp);
        }

        context.ExceptionHandled = true;

        ErrorViewModel model = new ErrorViewModel();
        model.Message = exp.Message;
        model.StackTrace = exp.StackTrace ?? string.Empty;

        if (exp.InnerException != null)
        {
            model.Message = model.Message + exp.InnerException.Message;
        }

        if (exp is StaleObjectStateException)
        {
            model.Message = "并发冲突，另一个操作已更改数据，或该操作已成功！请重试确认！";
        }

        var request = context.HttpContext.Request;
        if (WebUtil.ShouldShowJson(request))
        {
            context.Result = new JsonResult(new {
                success = false,
                message = model.Message,
            });
        }
        else
        {
            context.Result = new ViewResult() {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary()) {
                    Model = model
                }
            };
        }
    }
}