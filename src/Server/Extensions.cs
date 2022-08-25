namespace OpenAS2UI.Server
{
    internal static class Extensions
    {
        public static async Task<T[]> GetResult<T>(this HttpClient httpClient, string uri)
        {
            using Stream stream = await httpClient.GetStreamAsync(uri).ConfigureAwait(false);

            ResultList<T> resultList = new ResultList<T>(stream);
            resultList.ThrowIfError();

            return resultList.Results;
        }

        public static async Task<T[]> GetResult<T>(this HttpClient httpClient, HttpRequestMessage request)
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
            using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            ResultList<T> resultList = new ResultList<T>(stream);
            resultList.ThrowIfError();

            return resultList.Results;
        }
    }
}
