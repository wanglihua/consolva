using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Instruction;

public class InstructionArea {
    public static string Name => "Instruction";
}

public class InstructionAreaAttribute : AreaAttribute {
    public InstructionAreaAttribute() : base(InstructionArea.Name) {
    }
}