using EnvDTE;
using System;
using System.IO;
using System.Text;

//
//Created by exlted on 26-Apr-17. Edited by LordFratz on 27-Apr-17 - 2-May-17
//

public class PaletteListManager
{
    private string basePaletteList = "CoffeeScript|Resources/Droplet/example/palette/coffeescript_palette.coffee\\n" +
                                 "JavaScript|Resources/Droplet/example/palette/javascript_palette.coffee\\n" +
                                 "Python|Resources/Droplet/example/palette/Python_palette.coffee\\n" +
                                 "C|Resources/Droplet/example/palette/c_palette.coffee\\n" +
                                 "C++|Resources/Droplet/example/palette/c_c++_palette.coffee";

    private string paletteList;

    private static PaletteListManager PLM = null;

    private StringBuilder ForNewList;

    private Project CurrProj;

    private int ExampleNum = 1;

    private PaletteListManager()
    {
        paletteList = basePaletteList;
    }

    private void Traverse(DirectoryInfo dir, Project LocalProj)
    {
        foreach (FileInfo f in dir.GetFiles())
        {
            if(f.Name.EndsWith(".coffee"))
            {
                ForNewList.Append("\\n");
                int div = f.Name.LastIndexOf(".");
                ForNewList.Append(f.Name.Remove(div));
                ForNewList.Append('|');
                string tempConvert = f.FullName;
                tempConvert = tempConvert.Replace("\\", "/");
                ForNewList.Append(tempConvert);
                LocalProj.ProjectItems.AddFromFile(f.FullName);
            }
            if(f.Name.StartsWith("newPalette"))
            {
                string Num = f.Name.Substring(10);
                Num = Num.Replace(".coffee", "");
                int intNum;
                if(Int32.TryParse(Num, out intNum))
                {
                    if(intNum >= ExampleNum)
                    {
                        ExampleNum = intNum + 1;
                    }
                }
            }
        }
        foreach (DirectoryInfo d in dir.GetDirectories())
        {
            Traverse(d, LocalProj);
        }

    }

    public void updatePaletteList(Project proj)
    {
        CurrProj = proj;
        ForNewList = new StringBuilder(basePaletteList);
        string loc = proj.FullName;
        Project localProj = proj;
        DirectoryInfo DirInfo = new DirectoryInfo(localProj.FullName);
        if (DirInfo.Parent != null)
        {
            DirInfo = DirInfo.Parent;
        }
        string FullName = DirInfo.FullName;
        if (!Directory.Exists(FullName + "\\palettes"))
        {
            localProj.ProjectItems.AddFolder("palettes");
        }
        DirectoryInfo PalDir = new DirectoryInfo(FullName + "\\palettes");
        Traverse(PalDir, localProj);
        paletteList = ForNewList.ToString();
    }

    public string getPaletteList()
    {
        return paletteList;
    }

    public static PaletteListManager getPaletteListManager()
    {
        if (PLM == null)
        {
            PLM = new PaletteListManager();
        }
        return PLM;
    }

    public void AddNewPalette()
    {
        if (CurrProj != null)
        {
            updatePaletteList(CurrProj);
            string palette;
            using (StreamReader sr = new StreamReader("Resources/Droplet/example/palette/paletteExample.coffee"))
            {
                palette = sr.ReadToEnd();
            }
            DirectoryInfo DirInfo = new DirectoryInfo(CurrProj.FullName);
            if (DirInfo.Parent != null)
            {
                DirInfo = DirInfo.Parent;
            }
            string FullName = DirInfo.FullName;
            StreamWriter sw = new StreamWriter(FullName + "\\palettes\\newPalette" + ExampleNum + ".coffee");
            sw.Write(palette);
            sw.Close();
            updatePaletteList(CurrProj);
        }
    }
}