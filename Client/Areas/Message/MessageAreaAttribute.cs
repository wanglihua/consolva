using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Message;

public class MessageAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Message";

    public MessageAreaAttribute() : base(Name)
    {
    }
}