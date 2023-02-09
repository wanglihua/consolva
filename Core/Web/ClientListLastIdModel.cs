namespace shunda.Core.Web;

public class ClientListLastIdModel {
    /// <summary>
    /// 列表最后Id，被认为没有设置过值时的值
    /// </summary>
    public static int LAST_ID_NOT_SET_VALUE => 0;

    /// <summary>
    /// 列表最后Id，被认为列表已经没有更多值时的值
    /// </summary>
    public static int LAST_ID_NO_MORE_DATA_VALUE => 1;
}