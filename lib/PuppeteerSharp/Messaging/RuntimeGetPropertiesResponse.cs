using System.Collections.Generic;

namespace CefSharp.Puppeteer.Messaging
{
    internal class RuntimeGetPropertiesResponse
    {
        public IEnumerable<RuntimeGetPropertiesResponseItem> Result { get; set; }

        public class RuntimeGetPropertiesResponseItem
        {
            public object Enumerable { get; set; }

            public string Name { get; set; }

            public RemoteObject Value { get; set; }
        }
    }
}
