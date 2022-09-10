namespace shunda.Core;

[Serializable]
public class SexType
{
    public char Code { get; set; }
    public string Description { get; set; }

    private SexType(char code, string description)
    {
        this.Code = code;
        this.Description = description;
    }

    public static SexType Male = new SexType('M', "男");
    public static SexType Female = new SexType('F', "女");

    public static SexType GetSexByCode(char code)
    {
        if (code == Female.Code)
        {
            return Female;
        }

        if (code == Male.Code)
        {
            return Male;
        }

        throw new ArgumentException("Expect to be 'M' or 'F'.");
    }

    public static List<SexType> GetSexTypeList()
    {
        List<SexType> sexTypeList = new List<SexType>();
        sexTypeList.Add(Male);
        sexTypeList.Add(Female);

        return sexTypeList;
    }
}