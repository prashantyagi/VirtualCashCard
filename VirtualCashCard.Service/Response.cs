namespace VirtualCashCard.Service
{
    public class Response
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Response<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
