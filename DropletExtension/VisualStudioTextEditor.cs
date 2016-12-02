//------------------------------------------------------------------------------
// <copyright file="VisualStudioTextEditor.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.IO;
using System.Diagnostics;

namespace DropletExtension
{
    internal sealed class VisualStudioTextEditor
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        private DTE dte;

        private static string activeWindowFilePath;

        private string currentCodeLanguage = "CSharp";

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStudioTextEditor"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public VisualStudioTextEditor(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            layer = view.GetAdornmentLayer("VisualStudioTextEditor");

            this.view = view;
            this.view.LayoutChanged += OnLayoutChanged;

            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            dte.Events.WindowEvents.WindowActivated += OnWindowActivated;
        }

        public static void SetVSText()
        {
            // make sure droplet is open
            if (Droplet.Instance == null)
            {
                return;
            }

            // if droplet isn't active, then there shouldn't be any changes from there to move to VS
            if (Droplet.Instance.dropletEditorActive == true)
            {
                string dropletText = Droplet.Instance.dropletBrowser.GetText();
                if (dropletText != "")
                {
                    // Debug.WriteLine("SetVSText(): " + dropletText);
                    // this is the best way I know how to set the text in visual studio. not the best, but it works
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(activeWindowFilePath))
                        {
                            sw.Write(dropletText);
                        }
                    }
                    catch
                    {
                        // good programming right here (not really)
                    }
                }
            }
        }

        private void OnWindowActivated(Window GotFocus, Window LostFocus)
        {
            if (null != GotFocus.Document)
            {
                Document curDoc = GotFocus.Document;
                activeWindowFilePath = curDoc.FullName;

                // if droplet isn't open, then there's no point in doing anything else in this function
                if (Droplet.Instance == null)
                {
                    return;
                }

                // 
                ITextDocument tmpTextDocument;
                bool propertyNotNull = view.TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out tmpTextDocument);
                if (!propertyNotNull)
                {
                    return;
                }
                string tmpFilePath = tmpTextDocument.FilePath;


                // Check to see if programming language changes, and if it does, change the palette to the new language
                //string newCodeLanguage = curDoc.Language;

                //if (currentCodeLanguage != newCodeLanguage)
                //{
                //    currentCodeLanguage = newCodeLanguage;
                //    if (string.Compare(newCodeLanguage, "Python", true) == 0)
                //    {
                //        Droplet.Instance.dropletBrowser.chromeBrowser.LoadURL("http://localhost:8192/Resources/Droplet/example/example-python.html");
                //    }
                //    else if (string.Compare(newCodeLanguage, "CSharp", true) == 0)
                //    {
                //        Droplet.Instance.dropletBrowser.chromeBrowser.LoadURL("http://localhost:8192/Resources/Droplet/example/example.html");
                //    }
                //}

                // 
                if (Droplet.Instance.dropletEditorActive == false && string.Compare(activeWindowFilePath, tmpFilePath, true) == 0)
                {
                    string vsText = view.TextBuffer.CurrentSnapshot.GetText();
                    Debug.WriteLine("OnWindowActivated():\n" + vsText);
                    Droplet.Instance.dropletBrowser.SetText(vsText);
                }
            }
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            
            if (activeWindowFilePath == null)
            {
                return;
            }
            if (Droplet.Instance == null)
            {
                return;
            }
            if (e.OldSnapshot.GetText() == e.NewSnapshot.GetText())
            {
                return;
            }
            if (Droplet.Instance.dropletEditorActive == false)
            {
                Droplet.Instance.dropletBrowser.SetText(e.NewViewState.EditSnapshot.GetText());
            }

        }

    }
}
