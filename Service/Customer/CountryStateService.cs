using Quartz.Xml;
using shunda.Core.CodeType;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CountryStateService {
    public IList<StringCodeTypeBase> GetStateList(Country country) {
        if (country == Country.Australia) {
            return AustraliaState.GetList().ToList<StringCodeTypeBase>();
        }

        if (country == Country.America) {
            return AmericanState.GetList().ToList<StringCodeTypeBase>();
        }

        throw new ValidationException($"尚未支持的国家，{country.Name}");
    }
}