using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetBrowser;
using DotNetBrowser.WinForms;
using System.Net;
using System.Threading;
using System.IO;
using System.Web;
using DotNetBrowser.DOM;
using DotNetBrowser.DOM.Events;

namespace DropletExtension
{
    public partial class DropletBrowser : Form
    {
        public Browser chromeBrowser;

        public string result;

        public System.Diagnostics.Process server;

        public DropletBrowser()
        {
            InitializeComponent();

            this.Activated += DropletBrowser_Activated;
            this.Deactivate += DropletBrowser_Deactivate;

            InitializePythonServer();

            InitializeDotNetBrowser();
        }

        private void DropletBrowser_Deactivate(object sender, EventArgs e)
        {
            Droplet.Instance.dropletEditorActive = false;
        }

        private void DropletBrowser_Activated(object sender, EventArgs e)
        {
            Droplet.Instance.dropletEditorActive = true;
        }

        public void InitializeDotNetBrowser()
        {
            chromeBrowser = BrowserFactory.Create();
            BrowserView browserView = new WinFormsBrowserView(chromeBrowser);
            browserView.Browser.FinishLoadingFrameEvent += Browser_FinishLoadingFrameEvent;
            Controls.Add((Control)browserView);
            chromeBrowser.LoadURL("http://localhost:8888/Resources/Droplet/example/example.html");
        }

        private void Browser_FinishLoadingFrameEvent(object sender, DotNetBrowser.Events.FinishLoadingEventArgs e)
        {
            if (e.IsMainFrame)
            {
                Browser myBrowser = e.Browser;
                DOMDocument document = chromeBrowser.GetDocument();

                // I need to find the correct element and the right events to use in order to notice when text is changed on droplet
                DOMElement tmp = document.GetElementByClassName("droplet-wrapper-div");
                DOMElement keyChange = document.GetElementByClassName("droplet-wrapper-div");
                DOMElement dragCover = document.GetElementByClassName("droplet-drag-cover");
                DOMElement mouseUpTextEditor = document.GetElementByClassName("ace_content");
                DOMElement keyPressTextEditor = document.GetElementByClassName("ace_text-input");

                dragCover.AddEventListener(DOMEventType.OnMouseUp, DomEventHandlerOnMouseUp, true);
                mouseUpTextEditor.AddEventListener(DOMEventType.OnMouseUp, DomEventHandlerOnMouseUp, true);
                keyPressTextEditor.AddEventListener(DOMEventType.OnKeyDown, DomEventHandlerOnMouseUp, true);
                keyChange.AddEventListener(DOMEventType.OnKeyPress, DomEventHandlerOnMouseUp, true);
                tmp.AddEventListener(DOMEventType.OnMouseOver, DomEventHandlerOnMouseUp, true);
            }
        }

        private void DomEventHandlerOnMouseUp(object sender, DOMEventArgs e)
        {
            VisualStudioTextEditor.SetVSText();
        }

        public void InitializePythonServer()
        {
            server = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            //this should be set to hidden, but the process doesn't close properly if it is set to hidden
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C cd %LOCALAPPDATA%/Microsoft/VisualStudio/14.0/DropletExtension && python -m http.server 8888";
            server.StartInfo = startInfo;
            server.Start();
        }

        public string GetText()
        {
            string script = @"(function() 
                {
                return this.editor.getValue();
                })(); ";

            var tmp = chromeBrowser.ExecuteJavaScriptAndReturnValue(script);
            if (!tmp.IsNull())
            {
                result = tmp.GetString();
            }

            // for testing purposes only
            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\Users\jacky\Desktop\Testing Stuff\TestFromDroplet.txt"))
            //{
            //    sw.Write(result);
            //}

            return result;
        }

        public void SetText(string code)
        {
            string script = "this.editor.setValue(`" + code + "`);";

            chromeBrowser.ExecuteJavaScript(script);
        }
    }
}
