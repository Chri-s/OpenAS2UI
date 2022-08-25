namespace OpenAS2UI.Server
{

    [Serializable]
    public class ResultLoadingException : Exception
    {
        public ResultLoadingException() { }
        public ResultLoadingException(string message) : base(message) { }
        public ResultLoadingException(string message, Exception inner) : base(message, inner) { }
        protected ResultLoadingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
