using Microsoft.AspNetCore.Razor.TagHelpers;
using shunda.Core;

namespace shunda.Admin.TagHelpers;

[HtmlTargetElement("a", Attributes = "field, sort-field, sort-dir, sort-list-func")]
public class ListSortableFieldTagHelper : TagHelper
{
    public string Field { get; set; } = string.Empty;

    public string SortField { get; set; } = string.Empty;

    public string SortDir { get; set; } = string.Empty;

    public string SortListFunc { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        if (Field != SortField)
        {
            output.Attributes.SetAttribute("class", "list_sortable_field");
            output.Attributes.SetAttribute("href", string.Format("javascript:{0}('{1}', '{2}');", SortListFunc, Field, SortDirection.Asc));
            return;
        }

        if (SortDir == SortDirection.Asc)
        {
            output.Attributes.SetAttribute("class", "list_sortable_field_asc");
            output.Attributes.SetAttribute("href", string.Format("javascript:{0}('{1}', '{2}');", SortListFunc, Field, SortDirection.Desc));
        }
        else
        {
            output.Attributes.SetAttribute("class", "list_sortable_field_desc");
            output.Attributes.SetAttribute("href", string.Format("javascript:{0}('{1}', '{2}');", SortListFunc, Field, SortDirection.Asc));
        }
    }
}