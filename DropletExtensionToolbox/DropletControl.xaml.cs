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

        //public System.Diagnostics.Process server;

        bool serverOpen = false;

         

        private static string portNum = "6727";



        /// <summary>
        /// Initializes a new instance of the <see cref="DropletBrowser"/> class.
        /// </summary>
        public DropletBrowser()
        {
            this.InitializeComponent();

            //InitializePythonServer();


            InitializeDotNetBrowser();
        }

        // sets up the browser
        public void InitializeDotNetBrowser()
        {
            BrowserPreferences.SetChromiumSwitches("--remote-debugging-port=6727", "--disable-web-security", "--allow-file-access-from-files");
            chromeBrowser = BrowserFactory.Create();
            BrowserView browserView = new WPFBrowserView(chromeBrowser);
            browserView.Browser.FinishLoadingFrameEvent += Browser_FinishLoadingFrameEvent;
            dropletBrowser.Children.Add((UIElement)browserView.GetComponent());
            chromeBrowser.LoadURL(System.IO.Path.GetFullPath("Resources/Droplet/example/example.html"));
            System.Diagnostics.Debug.WriteLine(chromeBrowser.GetRemoteDebuggingURL());
        }


        // sets up the event handlers after the browser has been loaded
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

        }

        // sets the text from Droplet to Visual Studio after one of the Droplet events are triggered
        private void DomEventHandlerOnMouseUp(object sender, DOMEventArgs e)
        {
            VisualStudioTextEditor.SetVSText();
        }

        // returns the text that is in Droplet so that it can set the visual studio text
        public string GetText()
        {
            string result = string.Empty;

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

        // takes in the text from visual studio and pushes it into Droplet
        public void SetText(string code)
        {
            string script = "this.editor.setValue(`" + code + "`);";

            chromeBrowser.ExecuteJavaScript(script);
        }

        // these keep a bool of if Droplet has focus or not, so it won't receive anything from visual studio if Droplet is in focus, or vice versa
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