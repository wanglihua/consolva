using Newtonsoft.Json;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackWebHook;

public class SeventeenTrackWebHookRequestModel {
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;

    [JsonProperty("data")]
    public DataModel Data { get; set; } = new();

    public class DataModel {
        [JsonProperty("number")]
        public string Number { get; set; } = string.Empty;

        [JsonProperty("carrier")]
        public int Carrier { get; set; }

        [JsonProperty("track_info")]
        public TrackInfoModel TrackInfo { get; set; } = new();

        public class TrackInfoModel {
            [JsonProperty("latest_status")]
            public LatestStatusModel LatestStatus { get; set; } = new();

            public class LatestStatusModel {
                [JsonProperty("status")]
                public string Status { get; set; } = string.Empty;

                [JsonProperty("sub_status")]
                public string SubStatus { get; set; } = string.Empty;
            }

            [JsonProperty("tracking")]
            public TrackingModel Tracking { get; set; } = new();

            public class TrackingModel {
                [JsonProperty("providers")]
                public IList<ProviderListItem> ProviderList { get; set; } = new List<ProviderListItem>();

                public class ProviderListItem {
                    [JsonProperty("provider")]
                    public ProviderModel Provider { get; set; } = new();

                    public class ProviderModel {
                        [JsonProperty("key")]
                        public int Key { get; set; }

                        [JsonProperty("name")]
                        public string Name { get; set; } = string.Empty;
                    }

                    [JsonProperty("events")]
                    public IList<EventModel> EventList { get; set; } = new List<EventModel>();

                    public class EventModel {
                        [JsonProperty("time_iso")]
                        public string TimeIso { get; set; } = string.Empty;

                        [JsonProperty("description")]
                        public string Description { get; set; } = string.Empty;

                        [JsonProperty("stage")]
                        public string? Stage { get; set; }
                    }
                }
            }
        }
    }
}