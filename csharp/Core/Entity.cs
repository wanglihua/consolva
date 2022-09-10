using shunda.Core.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace shunda.Core;

public class Entity
{
    public virtual int Id { get; set; }

    [BindNever()]
    [JsonProperty("createTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime CreateTime { get; set; }

    [BindNever()]
    [JsonProperty("updateTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime UpdateTime { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is Entity == false)
        {
            return false;
        }

        if (Id == (obj as Entity)?.Id)
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