using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Instruction;

public class InstructionAreaAttribute : AreaAttribute {
    public static readonly string Name = "Instruction";

    public InstructionAreaAttribute() : base(Name) {
    }
}