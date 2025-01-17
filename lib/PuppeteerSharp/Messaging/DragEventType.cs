﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CefSharp.Puppeteer.Messaging
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum DragEventType
    {
        [EnumMember(Value = "dragEnter")]
        DragEnter,
        [EnumMember(Value = "dragOver")]
        DragOver,
        [EnumMember(Value = "drop")]
        Drop,
    }
}