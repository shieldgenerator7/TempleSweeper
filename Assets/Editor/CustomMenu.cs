using UnityEngine;
using System.Diagnostics;
using UnityEditor;

public class CustomMenu
{//2019-04-20: copied from Stonicorn.CustomMenu

    [MenuItem("SG7/Build/Build Windows %w")]
    public static void buildWindows()
    {
        build(BuildTarget.StandaloneWindows, "exe");
    }
    [MenuItem("SG7/Build/Build Linux %l")]
    public static void buildLinux()
    {
        build(BuildTarget.StandaloneLinux, "x86");
    }
    [MenuItem("SG7/Build/Build Mac OS X %#l")]
    public static void buildMacOSX()
    {
        build(BuildTarget.StandaloneOSX, "");
    }
    public static void build(BuildTarget buildTarget, string extension)
    {
        string defaultPath = getDefaultBuildPath();
        if (!System.IO.Directory.Exists(defaultPath))
        {
            System.IO.Directory.CreateDirectory(defaultPath);
        }
        //2017-10-19 copied from https://docs.unity3d.com/Manual/BuildPlayerPipeline.html
        // Get filename.
        string buildName = EditorUtility.SaveFilePanel("Choose Location of Built Game", defaultPath, PlayerSettings.productName, extension);

        // User hit the cancel button.
        if (buildName == "")
        {
            return;
        }

        string path = buildName.Substring(0, buildName.LastIndexOf("/"));
        UnityEngine.Debug.Log("BUILDNAME: " + buildName);
        UnityEngine.Debug.Log("PATH: " + path);

        string[] levels = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (EditorBuildSettings.scenes[i].enabled)
            {
                levels[i] = EditorBuildSettings.scenes[i].path;
            }
            else
            {
                break;
            }
        }

        // Build player.
        BuildPipeline.BuildPlayer(levels, buildName, buildTarget, BuildOptions.None);

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = buildName;
        proc.Start();
    }

    [MenuItem("SG7/Run/Run Windows %#w")]
    public static void runWindows()
    {//2018-08-10: copied from build()
        string extension = "exe";
        string buildName = getBuildNamePath(extension);
        UnityEngine.Debug.Log("Launching: " + buildName);
        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = buildName;
        proc.Start();
    }

    [MenuItem("SG7/Run/Open Build Folder #w")]
    public static void openBuildFolder()
    {
        string extension = "exe";
        string buildName = getBuildNamePath(extension);
        //Open the folder where the game is located
        EditorUtility.RevealInFinder(buildName);
    }

    public static string getDefaultBuildPath()
    {
        return "Builds/" + PlayerSettings.productName + "_" + PlayerSettings.bundleVersion.Replace(".", "_");
    }
    public static string getBuildNamePath(string extension, bool checkFolderExists = true)
    {
        string defaultPath = getDefaultBuildPath();
        if (checkFolderExists && !System.IO.Directory.Exists(defaultPath))
        {
            throw new UnityException("You need to build the " + extension + " for " + PlayerSettings.productName + " (Version " + PlayerSettings.bundleVersion + ") first!");
        }
        string buildName = defaultPath + "/" + PlayerSettings.productName + "." + extension;
        return buildName;
    }
}
