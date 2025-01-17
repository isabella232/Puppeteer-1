using System;
using System.Collections.Generic;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// <see cref="DevToolsContext.Dialog"/> arguments.
    /// </summary>
    public class DialogEventArgs : EventArgs
    {
        /// <summary>
        /// Dialog data.
        /// </summary>
        public Dialog Dialog { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEventArgs"/> class.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        public DialogEventArgs(Dialog dialog) => Dialog = dialog;
    }
}