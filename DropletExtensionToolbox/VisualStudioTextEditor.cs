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
using System.Timers;

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
        private IWpfTextView view;

        private DTE dte;

        private static string activeWindowFilePath;

        public static string currentCodeLanguage;

        public static string currentCodeText;

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

            // if droplet is already open, then set the text right away
            currentCodeText = view.TextBuffer.CurrentSnapshot.GetText();
            if (DropletCommand.Instance != null)
            {
                DropletCommand.Instance.dropletBrowser.DropletBrowser.SetText(view.TextBuffer.CurrentSnapshot.GetText());
            }
        }

        // reads in the text from droplet to set the visual studio text
        public static void SetVSText()
        {
            // make sure droplet is open
            if (DropletCommand.Instance == null)
            {
                return;
            }

            // if droplet isn't active, then there shouldn't be any changes from there to move to VS
            if (DropletCommand.Instance.dropletEditorActive == true)
            {
                string dropletText = DropletCommand.Instance.dropletBrowser.DropletBrowser.GetText();
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

        // when each visual studio file is opened, this function is called
        // function changes Droplet's programming language and text to whatever the new open file is
        private void OnWindowActivated(Window GotFocus, Window LostFocus)
        {
            if (null != GotFocus.Document)
            {
                Document curDoc = GotFocus.Document;
                activeWindowFilePath = curDoc.FullName;

                // if droplet isn't open, then there's no point in doing anything else in this function
                if (DropletCommand.Instance == null)
                {
                    return;
                }

                // get the file path to make sure the current document is the same as the file it's looking for
                ITextDocument tmpTextDocument;
                bool propertyNotNull = view.TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out tmpTextDocument);
                if (!propertyNotNull)
                {
                    return;
                }
                string tmpFilePath = tmpTextDocument.FilePath;
                if (string.Compare(activeWindowFilePath, tmpFilePath, true) != 0)
                {
                    return;
                }


                // Check to see if programming language changes, and if it does, change the palette to the new language
                string newCodeLanguage = curDoc.Language;

                if (newCodeLanguage == "C/C++")
                {
                    newCodeLanguage = "c_c++";
                }

                // currentCodeLanguage keeps getting changed outside of this function, and I can't tell where
                if (currentCodeLanguage != newCodeLanguage)
                {
                    currentCodeLanguage = newCodeLanguage;

                    // read the code from the given palette file for the language
                    string script = string.Empty;
                    string palette = string.Empty;
                    // idk if this file path will work for others, or just me
                    string filePath = "Resources/Droplet/example/palette/" + newCodeLanguage + "_palette.coffee";

                    try
                    {
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            palette = sr.ReadToEnd();
                        }
                    }
                    catch
                    {
                        // That programming language isn't supported yet
                    }


                    // push that code into palette, then update palette
                    script = "this.localStorage.setItem('config', `" + palette + "`); update.click();";

                    // then update the code shown in droplet
                    script += "this.editor.setValue(`" + view.TextBuffer.CurrentSnapshot.GetText() + "`); toggle.click();";

                    DropletCommand.Instance.dropletBrowser.DropletBrowser.chromeBrowser.ExecuteJavaScript(script);

                    DropletCommand.Instance.dropletBrowser.DropletBrowser.Browser_FinishLoadingFrameEvent(null, null);
                }
            }
        }

        // anytime the layout changes (scrolling/typing stuff) this function is called
        // error checking to make sure the droplet text should be updated, and updates the text
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // make sure things are open
            if (activeWindowFilePath == null)
            {
                return;
            }
            if (DropletCommand.Instance == null)
            {
                return;
            }
            // make sure text was changed
            if (e.OldSnapshot.GetText() == e.NewSnapshot.GetText())
            {
                return;
            }
            // set the new text
            if (DropletCommand.Instance.dropletEditorActive == false)
            {
                currentCodeText = e.NewViewState.EditSnapshot.GetText();
                DropletCommand.Instance.dropletBrowser.DropletBrowser.SetText(e.NewViewState.EditSnapshot.GetText());
            }

        }

    }
}
