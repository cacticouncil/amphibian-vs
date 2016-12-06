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
            chromeBrowser.LoadURL("http://localhost:3426/Resources/Droplet/example/example.html");
        }

        private void Browser_FinishLoadingFrameEvent(object sender, DotNetBrowser.Events.FinishLoadingEventArgs e)
        {
            DOMDocument document = chromeBrowser.GetDocument();

            // Sets event listeners to certain parts of the html page, so changes can be recognized and sent to the Visual Studio editor
            DOMElement keyChange = document.GetElementByClassName("droplet-wrapper-div");
            List<DOMNode> keyPressTextEditor = document.GetElementsByClassName("ace_text-input");

            for (int i = 0; i < keyPressTextEditor.Count; i++)
            {
                keyPressTextEditor.ElementAt(i).AddEventListener(DOMEventType.OnKeyUp, DomEventHandlerOnMouseUp, true);
            }
            keyChange.AddEventListener(DOMEventType.OnMouseOver, DomEventHandlerOnMouseUp, true);


            //server.CloseMainWindow();
            //server.Close();
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
            startInfo.Arguments = "/C cd %LOCALAPPDATA%/Microsoft/VisualStudio/14.0/DropletExtension && python -m http.server 3426";
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
