using Microsoft.AspNetCore.Mvc;

namespace shunda.Core.Web;

public class IdListParam
{
    [BindProperty(Name = "idList")]
    public IList<int> IdList { get; set; } = new List<int>();

    public void Validate(string message = "请选择要操作的数据项！")
    {
        if (IdList.Count == 0)
        {
            throw new ValidationException(message);
        }
    }
}