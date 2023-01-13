using shunda.Core;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientInstruction;

public class ClientInstructionListModel : ListModel {
    public ClientInstructionListModel(ListPager listPager) : base(listPager) {
    }

    public IList<ClientInstructionInfo> ClientInstructionList { get; set; } = new List<ClientInstructionInfo>();
}