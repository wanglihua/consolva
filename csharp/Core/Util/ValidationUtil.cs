using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.Util;

public static class ValidationUtil
{
    public static string GetErrorMessages(ModelStateDictionary modelState)
    {
        StringBuilder sb = new StringBuilder();

        foreach (KeyValuePair<string, ModelStateEntry> entry in modelState)
        {
            foreach (ModelError modelError in entry.Value.Errors)
            {
                sb.AppendFormat("{0}<br />{1}", modelError.ErrorMessage, Environment.NewLine);
            }
        }

        return sb.ToString();
    }

    public static string GetErrorMessagesForMobile(ModelStateDictionary modelState)
    {
        StringBuilder sb = new StringBuilder();

        foreach (KeyValuePair<string, ModelStateEntry> entry in modelState)
        {
            foreach (ModelError modelError in entry.Value.Errors)
            {
                sb.AppendFormat("{0}{1}", modelError.ErrorMessage, Environment.NewLine);
            }
        }

        return sb.ToString();
    }

    public static string GetErrorMessages(IList<string> messageList)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var message in messageList)
        {
            sb.AppendFormat("{0}<br />{1}", message, Environment.NewLine);
        }

        return sb.ToString();
    }

    public static string GetErrorMessagesForMobile(IList<string> messageList)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var message in messageList)
        {
            sb.AppendFormat("{0}{1}", message, Environment.NewLine);
        }

        return sb.ToString();
    }
}