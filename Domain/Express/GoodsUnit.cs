using System.Collections.Generic;

namespace shunda.Domain.Express;

public class GoodsUnit
{
    public static IList<string> GetList()
    {
        IList<string> unitList = new List<string>() {
            "个",
            "件",
            "台",
            "套",
            "双",
            "只",
            "张",
            "箱",
            "本",
            "包",
            "盒",
            "卷",
            "拖"
        };

        return unitList;
    }
}