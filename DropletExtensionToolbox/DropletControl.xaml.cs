//------------------------------------------------------------------------------
// <copyright file="DropletBrowser.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace DropletExtension
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using DotNetBrowser;
    using DotNetBrowser.WPF;
    using DotNetBrowser.DOM;
    using System.Collections.Generic;
    using DotNetBrowser.DOM.Events;
    using System.Linq;

    /// <summary>
    /// Interaction logic for DropletBrowser.
    /// </summary>
    public partial class DropletBrowser : UserControl
    {
        public Browser chromeBrowser;

        public System.Diagnostics.Process server;

        bool serverOpen = false;

        public string result;

        private static string portNum = "6719";



        /// <summary>
        /// Initializes a new instance of the <see cref="DropletBrowser"/> class.
        /// </summary>
        public DropletBrowser()
        {
            this.InitializeComponent();

            InitializePythonServer();

            InitializeDotNetBrowser();


 
        }


        public void InitializeDotNetBrowser()
        {
            chromeBrowser = BrowserFactory.Create();
            BrowserView browserView = new WPFBrowserView(chromeBrowser);
            browserView.Browser.FinishLoadingFrameEvent += Browser_FinishLoadingFrameEvent;
            dropletBrowser.Children.Add((UIElement)browserView.GetComponent());
            chromeBrowser.LoadURL("http://localhost:" + portNum + "/example/example.html");
        }
        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        public void InitializePythonServer()
        {
            server = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            serverOpen = true;

            //this should be set to hidden, but the process doesn't close properly if it is set to hidden
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            // this is very hardcoded for my pc, so I need to fix it
            startInfo.Arguments = "/C cd Resources/Droplet && python -m http.server " + portNum;
            server.StartInfo = startInfo;
            server.Start();
        }



        public void Browser_FinishLoadingFrameEvent(object sender, DotNetBrowser.Events.FinishLoadingEventArgs e)
        {
            DOMDocument document = chromeBrowser.GetDocument();

            // Sets event listeners to certain parts of the html page, so changes can be recognized and sent to the Visual Studio editor
            List<DOMNode> mouseDownWrapperDiv = document.GetElementsByClassName("droplet-wrapper-div");
            List<DOMNode> keyPressTextEditor = document.GetElementsByClassName("ace_text-input");

            for (int i = 0; i < keyPressTextEditor.Count; i++)
            {
                keyPressTextEditor.ElementAt(i).AddEventListener(DOMEventType.OnKeyUp, DomEventHandlerOnMouseUp, false);
            }
            for (int i = 0; i < mouseDownWrapperDiv.Count; i++)
            {
                mouseDownWrapperDiv.ElementAt(i).AddEventListener(DOMEventType.OnMouseMove, DomEventHandlerOnMouseUp, false);
                mouseDownWrapperDiv.ElementAt(i).AddEventListener(DOMEventType.OnKeyUp, DomEventHandlerOnMouseUp, false);
            }

            if (serverOpen)
            {
                server.CloseMainWindow();
                server.Close();
                serverOpen = false;
            }
            
        }

        private void DomEventHandlerOnMouseUp(object sender, DOMEventArgs e)
        {
            VisualStudioTextEditor.SetVSText();
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


            return result;
        }

        public void SetText(string code)
        {
            string script = "this.editor.setValue(`" + code + "`);";

            chromeBrowser.ExecuteJavaScript(script);
        }

        private void dropletBrowser_GotFocus(object sender, RoutedEventArgs e)
        {
            DropletCommand.Instance.dropletEditorActive = true;
        }

        private void dropletBrowser_LostFocus(object sender, RoutedEventArgs e)
        {
            DropletCommand.Instance.dropletEditorActive = false;
        }


    }
}