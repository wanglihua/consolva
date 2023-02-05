using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Message;

public class MessageArea {
    public static string Name => "Message";
}

public class MessageAreaAttribute : AreaAttribute {
    public MessageAreaAttribute() : base(MessageArea.Name) {
    }
}