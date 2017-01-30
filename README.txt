The Toolbox branch will be the most updated version of the plugin. We should probably have the toolbox version in the master branch. 

In order for the plugin to work properly, Python 3 needs to be installed, including adding Python 3 to environment variables.

Most of the plugin code is in DropletControl.xaml.cs and VisualStudioTextEditor.cs, with DropletControl being used for managing the local web server and browser that contains droplet itself, and VisualStudioTextEditor being used to manage the visual studio side of things. 

Droplet Extention uses DotNetBrowser http://www.teamdev.com/dotnetbrowser, which is a proprietary software. The use of DotNetBrowser is governed by DotNetBrowser Product Licence Agreement http://www.teamdev.com/dotnetbrowser-licence-agreement. If you would like to use DotNetBrowser in your development, please contact TeamDev.