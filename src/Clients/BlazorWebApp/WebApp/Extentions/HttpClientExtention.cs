namespace WebApp.Extentions;

public static class HttpClientExtention
{
    public async static Task<TResult> PostGetResponseAsync<TResult, TValue>(this HttpClient client, String url, TValue value)
    {
        var httRes = await client.PostAsJsonAsync(url, value);

        return httRes.IsSuccessStatusCode ? await httRes.Content.ReadFromJsonAsync<TResult>() : default;
        
    }
    public async static Task PostAsync<TValue>(this HttpClient httpClient, String url, TValue value)
    {
        await httpClient.PostAsJsonAsync(url, value);
    }
    public async static Task<T> GetResponseAsync<T>(this HttpClient httpClient, String url)
    {
        return await httpClient.GetFromJsonAsync<T>(url);
    }
}
