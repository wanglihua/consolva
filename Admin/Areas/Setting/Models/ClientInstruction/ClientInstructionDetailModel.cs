using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientInstruction;

public class ClientInstructionDetailModel : ClientInstructionSaveModel {
    [BindNever]
    public IList<ClientInstructionType> TypeList { get; set; } = ClientInstructionType.GetList();
}