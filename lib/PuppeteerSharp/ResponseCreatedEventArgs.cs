using System;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// <see cref="DevToolsContext.Response"/> arguments.
    /// </summary>
    public class ResponseCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>The response.</value>
        public Response Response { get; internal set; }
    }
}