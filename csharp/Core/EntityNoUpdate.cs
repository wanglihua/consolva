using shunda.Core.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace shunda.Core;

public class EntityNoUpdate
{
    public virtual int Id { get; set; }

    [BindNever()]
    [JsonProperty("createTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime CreateTime { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is EntityNoUpdate == false)
        {
            return false;
        }

        if (Id == (obj as EntityNoUpdate)?.Id)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (Id == 0)
        {
            return base.GetHashCode();
        }

        return Id.GetHashCode();
    }
}