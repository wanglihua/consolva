using shunda.Core.CodeType;

namespace shunda.Domain;

public class Country : CodeTypeBase
{
    /// <summary>
    /// 中国, 1000
    /// </summary>
    public static readonly Country China = new Country(1000, "中国", "中国");

    /// <summary>
    /// 澳洲, 2000
    /// </summary>
    public static readonly Country Australia = new Country(2000, "澳洲", "澳洲");

    /// <summary>
    /// 美国, 3000
    /// </summary>
    public static readonly Country America = new Country(3000, "美国", "美国");

    /// <summary>
    /// None
    /// </summary>
    public static readonly Country None = new Country(0, "", "");


    public static Country GetById(int id)
    {
        IList<Country> countryList = GetList();
        foreach (Country country in countryList)
        {
            if (country.Id == id)
            {
                return country;
            }
        }

        return None;
    }

    public static IList<Country> GetList()
    {
        IList<Country> countryList = new List<Country>();
        countryList.Add(China);
        countryList.Add(Australia);

        return countryList;
    }

    private Country(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CountryHibernateType : CodeHibernateTypeBase<Country>
{
    protected override Country GetCodeTypeById(int id)
    {
        return Country.GetById(id);
    }
}

public class CountryModelBinder : CodeTypeModelBinderBase<Country>
{
    protected override Country GetCodeTypeById(int id)
    {
        return Country.GetById(id);
    }
}