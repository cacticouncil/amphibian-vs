In order for the plugin to work properly, Python 3 needs to be installed, including adding Python 3 to environment variables.

Most of the plugin code is in DropletControl.xaml.cs and VisualStudioTextEditor.cs, with DropletControl being used for managing the local web server and browser that contains droplet itself, and VisualStudioTextEditor being used to manage the visual studio side of things. 

Droplet itself is in Resources so that the local web server can find the correct files on the user's computer. For updating Droplet, all that you'd really need to change are the files in the dist folder, although example/example.coffee may need to be changed too, if it doesn't work properly. If example/example.coffee does need to be changed, the only lines that'd need to be changed are lines 75 and 75. 
The files in example/palette are the actual palette files needed for droplet to change programming languages. 

Droplet Extention uses DotNetBrowser http://www.teamdev.com/dotnetbrowser, which is a proprietary software. The use of DotNetBrowser is governed by DotNetBrowser Product Licence Agreement http://www.teamdev.com/dotnetbrowser-licence-agreement. If you would like to use DotNetBrowser in your development, please contact TeamDev.