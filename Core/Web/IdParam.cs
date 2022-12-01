using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Core.Web;

public class IdParam
{
    [BindProperty(Name = "id")]
    [Required(ErrorMessage = "id不能为空！")]
    public int Id { get; set; } = 0;
}