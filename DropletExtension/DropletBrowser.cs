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
using System.Diagnostics;

namespace DropletExtension
{
    public partial class DropletBrowser : Form
    {
        public Browser chromeBrowser;

        public string result;

        public Process server;

        private static string portNum = "3444";

        private bool isOpen = false;

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
            chromeBrowser.LoadURL("http://localhost:" + portNum + "/Resources/Droplet/example/example.html");
        }

        public void Browser_FinishLoadingFrameEvent(object sender, DotNetBrowser.Events.FinishLoadingEventArgs e)
        {
            DOMDocument document = chromeBrowser.GetDocument();

            // Sets event listeners to certain parts of the html page, so changes can be recognized and sent to the Visual Studio editor
            List<DOMNode> mouseDownWrapperDiv = document.GetElementsByClassName("droplet-wrapper-div");
            List<DOMNode> keyPressTextEditor = document.GetElementsByClassName("ace_text-input");

            for (int i = 0; i < keyPressTextEditor.Count; i++)
            {
                keyPressTextEditor.ElementAt(i).AddEventListener(DOMEventType.OnKeyUp, DomEventHandlerOnMouseUp, true);
            }
            for (int i = 0; i < mouseDownWrapperDiv.Count; i++)
            {
                mouseDownWrapperDiv.ElementAt(i).AddEventListener(DOMEventType.OnMouseMove, DomEventHandlerOnMouseUp, true);
                mouseDownWrapperDiv.ElementAt(i).AddEventListener(DOMEventType.OnKeyPress, DomEventHandlerOnMouseUp, true);
            }

            if (VisualStudioTextEditor.currentCodeText != null && isOpen == false)
            {
                SetText(VisualStudioTextEditor.currentCodeText);
                isOpen = true;
            }
        }



        private void DomEventHandlerOnMouseUp(object sender, DOMEventArgs e)
        {
            VisualStudioTextEditor.SetVSText();
        }

        public void InitializePythonServer()
        {
            server = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            //this should be set to hidden, but the process doesn't close properly if it is set to hidden
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C cd %LOCALAPPDATA%/Microsoft/VisualStudio/14.0/DropletExtension && python -m http.server " + portNum;
            server.StartInfo = startInfo;
            server.Start();
        }

        public string GetText()
        {
            string script = @"(function() 
                {
                return this.editor.getValue();
                })(); ";

            var tmpResult = chromeBrowser.ExecuteJavaScriptAndReturnValue(script);
            if (!tmpResult.IsNull())
            {
                result = tmpResult.GetString();
            }

            return result;
        }

        public void SetText(string code)
        {
            code = code.Replace("`", "\\`");
            string script = "this.editor.setValue(`" + code + "`);";

            chromeBrowser.ExecuteJavaScript(script);
        }
    }
}
