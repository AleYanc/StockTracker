namespace StockTracker.Wrappers
{
    public class Response<T>
    {
        public Response() { }

        public Response(T response) 
        {
            Succeeded = true;
        }

        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
    }
}
