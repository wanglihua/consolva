namespace shunda.Admin.Models.UserPassRes;

public class UserPassResListItem
{
    public int PassResId { get; set; }

    public string CateName { get; set; } = string.Empty;

    public string ResName { get; set; } = string.Empty;

    public bool Granted { get; set; }
}