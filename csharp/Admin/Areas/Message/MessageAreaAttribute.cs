using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Message;

public class MessageAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Message";

    public MessageAreaAttribute() : base(Name)
    {
    }
}