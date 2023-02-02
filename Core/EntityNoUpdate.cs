using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using shunda.Core.Json;

namespace shunda.Core;

public abstract class EntityNoUpdate : EntityNoCreateNoUpdate {
    [BindNever()]
    [JsonProperty("createTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime CreateTime { get; set; }
}