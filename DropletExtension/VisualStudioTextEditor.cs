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
using System.Windows.Forms;
using DotNetBrowser.DOM.Events;

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
            if (Droplet.Instance == null)
            {
                return;
            }
            if (Droplet.Instance.dropletEditorActive == true)
            {
                string dropletText = Droplet.Instance.dropletBrowser.GetText();
                if (dropletText != "")
                {
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
                if (Droplet.Instance == null)
                {
                    return;
                }

                ITextDocument tmpTextDocument;
                bool propertyNotNull = view.TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out tmpTextDocument);
                if (!propertyNotNull)
                {
                    return;
                }
                string tmpFilePath = tmpTextDocument.FilePath;

                if (Droplet.Instance.dropletEditorActive == false && string.Compare(activeWindowFilePath, tmpFilePath, true) == 0)
                {
                    Droplet.Instance.dropletBrowser.SetText(view.TextBuffer.CurrentSnapshot.GetText());
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
