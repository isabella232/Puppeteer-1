namespace CefSharp.Puppeteer
{
    /// <summary>
    /// <see cref="DevToolsContext.FrameAttached"/>, <see cref="DevToolsContext.FrameDetached"/> and <see cref="DevToolsContext.FrameNavigated"/> arguments.
    /// </summary>
    public class FrameEventArgs
    {
        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public Frame Frame { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameEventArgs"/> class.
        /// </summary>
        /// <param name="frame">Frame.</param>
        public FrameEventArgs(Frame frame)
        {
            Frame = frame;
        }
    }
}