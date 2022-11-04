using System.Net;
using Newtonsoft.Json;
using shunda.Core.Service;

namespace shunda.Core;

[Service(ServiceType.Scoped)]
public class HttpRequestService
{
    private static readonly TimeSpan waitTime = new TimeSpan(0, 10, 0);


    public T GetJsonObject<T>(string url)
    {
        return GetJsonObject<T>(new Dictionary<string, string>(), url);
    }

    public T GetJsonObject<T>(Dictionary<string, string> headers, string url)
    {
        string json = GetJson(headers, url);

        if (string.IsNullOrWhiteSpace(json))
        {
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(json)!;
    }

    public string GetJson(string url)
    {
        return GetJson(new Dictionary<string, string>(), url);
    }

    public string GetJson(Dictionary<string, string> headers, string url)
    {
        HttpClient httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
        HttpResponseMessage response = null;
        try
        {
            httpClient.CancelPendingRequests();
            httpClient.DefaultRequestHeaders.Clear();

            if (headers != null && headers.Count != 0)
            {
                foreach (KeyValuePair<string, string> keyValuePair in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(keyValuePair.Key, new string[] { keyValuePair.Value });
                }
            }

            Task<HttpResponseMessage> taskResponse = httpClient.GetAsync(url);
            taskResponse.Wait(waitTime);
            response = taskResponse.Result;

            if (response.IsSuccessStatusCode)
            {
                Task<Stream> taskStream = response.Content.ReadAsStreamAsync();
                taskStream.Wait(waitTime);
                Stream dataStream = taskStream.Result;
                StreamReader reader = new StreamReader(dataStream);
                string result = reader.ReadToEnd();

                return result;
            }
        }
        finally
        {
            if (response != null)
            {
                response.Dispose();
            }

            httpClient.Dispose();
        }

        return string.Empty;
    }
}