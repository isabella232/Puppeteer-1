namespace CefSharp.Puppeteer.Messaging
{
    internal class ResponseReceivedResponse
    {
        public string RequestId { get; set; }

        public ResponsePayload Response { get; set; }
    }
}
