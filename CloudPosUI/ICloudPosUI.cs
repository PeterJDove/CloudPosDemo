using System;

/// <summary>
/// This small namespace provides an Interface that serves as the base for <b>UI</b> classes implemented elsewhere.
/// </summary>
namespace Touch.CloudPosUI
{
    /// <summary>
    /// This Interface provides the base for <b>UI</b> classes which encapsulate a Browser.
    /// </summary>
    public interface ICloudPosUI : IDisposable
    {
        /// <summary>
        /// To be raised when the javascript application sends a message to be handled by the POS.
        /// </summary>
        event EventHandler<string> Notify;

        /// <summary>
        /// To be raised when the web browser has loaded the HTML web application.
        /// </summary>
        event EventHandler<string> Loaded;

        /// <summary>
        /// To be raised when the GUI window is closed.
        /// </summary>
        event EventHandler Unloaded;

        /// <summary>
        /// Sets the position, on the screen, of the CloudPOS GUI window.
        /// </summary>
        /// <param name="left">The pixel position of the left of the GUI window</param>
        /// <param name="top">The pixel position of the top of the GUI window</param>
        void SetPosition(int left, int top);

        /// <summary>
        /// Sets the size of the CloudPOS GUI window.
        /// </summary>
        /// <param name="width">The window width, in pixels</param>
        /// <param name="height">The window height, in pixels</param>
        void SetClientSize(int width, int height);

        /// <summary>
        /// Show the CloudPOS GUI window.
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the CloudPOS GUI window.
        /// </summary>
        void Hide();

        /// <summary>
        /// Instructs the browser encapsulated in the CloudPOS GUI to load a web page (a web application).
        /// </summary>
        /// <param name="url">The address of the web page to load</param>
        void Navigate(string url);

        /// <summary>
        /// Sends a JSON command into CloudPOS; i.e. into the javascript application ruuning in the browser.
        /// </summary>
        /// <param name="json"></param>
        void SendMessage(string json);
    }
}
