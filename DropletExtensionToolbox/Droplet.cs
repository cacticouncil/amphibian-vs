//------------------------------------------------------------------------------
// <copyright file="Droplet.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace DropletExtension
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("94e4f1e4-1455-4c65-985f-d67e7a9ce218")]
    public class Droplet : ToolWindowPane
    {

        public DropletBrowser DropletBrowser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Droplet"/> class.
        /// </summary>
        public Droplet() : base(null)
        {
            this.Caption = "Droplet";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new DropletBrowser();
            DropletBrowser = (DropletBrowser)Content;
        }
    }
}
