using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CefSharp.Puppeteer.Messaging
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum DispatchKeyEventType
    {
        [EnumMember(Value = "keyDown")]
        KeyDown,
        [EnumMember(Value = "rawKeyDown")]
        RawKeyDown,
        [EnumMember(Value = "keyUp")]
        KeyUp,
    }
}