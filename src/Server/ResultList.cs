using System.Text.Json;

namespace OpenAS2UI.Server
{
    public class ResultList<T>
    {
        public ResultList(Stream stream)
        {
            // Because results can have different content depending on the type is "OK", "ERROR" or "EXCEPTION"
            // we can't use the normal JsonSerialization and must serialize on our own.
            using JsonDocument doc = JsonDocument.Parse(stream);

            this.Type = doc.RootElement.GetProperty("type").GetString() ?? string.Empty;
            this.Result = doc.RootElement.GetProperty("result").GetString() ?? string.Empty;

            if (this.Type == "OK")
            {
                this.Results = JsonSerializer.Deserialize<T[]>(doc.RootElement.GetProperty("results")) ?? new T[0];
            }
        }

        public string Type { get; set; } = string.Empty;

        public T[] Results { get; set; } = new T[0];

        public string Result { get; set; } = string.Empty;

        public void ThrowIfError()
        {
            if (this.Type == "OK")
                return;

            throw new ResultLoadingException(this.Result);
        }

        public bool IsError(out string error)
        {
            error = string.Empty;

            if (this.Type == "OK")
                return false;

            error = this.Result;
            return true;
        }
    }
}
