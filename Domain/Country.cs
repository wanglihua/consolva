using shunda.Core.CodeType;

namespace shunda.Domain;

public class Country : CodeTypeBase {
    /// <summary>
    /// 中国, 1000
    /// </summary>
    public static readonly Country China = new Country(1000, "China", "中国", "中国");

    /// <summary>
    /// 澳大利亚, 2000
    /// </summary>
    public static readonly Country Australia = new Country(2000, "Australia", "澳大利亚", "澳大利亚");

    /// <summary>
    /// 美国, 3000
    /// </summary>
    public static readonly Country America = new Country(3000, "America", "美国", "美国");

    /// <summary>
    /// 加拿大, 4000
    /// </summary>
    public static readonly Country Canada = new Country(4000, "Canada", "加拿大", "加拿大");

    /// <summary>
    /// 新西兰, 6000
    /// </summary>
    public static readonly Country NewZealand = new Country(6000, "New Zealand", "新西兰", "新西兰");

    /// <summary>
    /// 新加坡, 7000
    /// </summary>
    public static readonly Country Singapore = new Country(7000, "Singapore", "新加坡", "新加坡");

    /// <summary>
    /// 马来西亚, 8000
    /// </summary>
    public static readonly Country Malaysia = new Country(8000, "Malaysia", "马来西亚", "马来西亚");

    /*
    暂时不开放

    /// <summary>
    /// 英国, 5000
    /// </summary>
    public static readonly Country Britain = new Country(5000, "Britain", "英国", "英国");

    /// <summary>
    /// 日本, 9000
    /// </summary>
    public static readonly Country Japan = new Country(9000, "Japan", "日本", "日本");

    /// <summary>
    /// 泰国, 10000
    /// </summary>
    public static readonly Country Thailand = new Country(10000, "Thailand", "泰国", "泰国");

    /// <summary>
    /// 印度尼西亚, 11000
    /// </summary>
    public static readonly Country Indonesia = new Country(11000, "Indonesia", "印度尼西亚", "印度尼西亚");
    
    */


    /// <summary>
    /// None
    /// </summary>
    public static readonly Country None = new Country(0, "", "", "");

    private static readonly IReadOnlyList<Country> codeTypeList = new List<Country>() {
        China,
        Australia,
        America,
        Canada,
        NewZealand,
        Singapore,
        Malaysia,

        /*
        Britain,
        Japan,
        Thailand,
        Indonesia,
        */
    };

    private static readonly IDictionary<int, Country> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static Country GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out Country? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<Country> GetList() {
        return codeTypeList.ToList();
    }

    public static IList<Country> GetNoChinaList() {
        return codeTypeList.Where(e => e != China).ToList();
    }

    /// <summary>
    /// 英文名称
    /// </summary>
    public string EnglishName { get; set; } = string.Empty;

    private Country(int id, string englishName, string name, string description) : base(id, name, description) {
        this.EnglishName = englishName;
    }
}

public class CountryHibernateType : CodeHibernateTypeBase<Country> {
    protected override Country GetCodeTypeById(int id) {
        return Country.GetById(id);
    }
}

public class CountryModelBinder : CodeTypeModelBinderBase<Country> {
    protected override Country GetCodeTypeById(int id) {
        return Country.GetById(id);
    }
}