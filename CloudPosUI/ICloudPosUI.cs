using System;

/*
 *  This Interface provides the base for "UI" classes 
 *  which encapsulate a Browser, in:
 *   - CloudPosIE           (Internet Explorer)
 *   - CloudPosAwesomium    (Awesomium Browser)
 *   - CloudPosEO           (Essential Objects)
 */ 
namespace CloudPosUI
{
    public interface ICloudPosUI : IDisposable
    {
        event EventHandler<string> Notify; // message events from the javascript 
        event EventHandler<string> Loaded;
        event EventHandler Unloaded;

        void SetPosition(int left, int top);
        void SetClientSize(int width, int height);
        void Show();
        void Hide();
        void Navigate(string url);
        void SendMessage(string json);
    }
}
