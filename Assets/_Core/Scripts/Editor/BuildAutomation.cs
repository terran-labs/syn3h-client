using System;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

public class BuildAutomation
{
    [MenuItem("Automation/Build All, With Installers", false, 10)]
    static void BuildAll()
    {
        Debug.Log("BuildAutomation :: BUILD ALL :: Initiated");

        PrepareToBuild();

        BuildWin();
        BuildMac();
        BuildLinux();
        //BuildAndroid();
        //BuildIOS();

        PackageInstallers();

        Debug.Log("BuildAutomation :: BUILD ALL :: Finished (check output for each platform to confirm success)");
    }

    static void PrepareToBuild()
    {
        Debug.Log("BuildAutomation :: Preparing to Build...");

        // @todo - Download latest JSON file, set version #, etc  
    }

    [MenuItem("Automation/Package Installers", false, 30)]
    static void PackageInstallers()
    {
        Debug.Log("BuildAutomation :: Building Installer Packages...");

        Process p = new Process();
        ProcessStartInfo info = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
        info.UseShellExecute = true;
        info.CreateNoWindow = false;
        info.WorkingDirectory = Application.dataPath + "\\..\\";
        info.FileName = "Config\\build_installer_packages.bat";
        info.Arguments = "";
        p.StartInfo = info;
        p.Start();

        Debug.LogError("A command window has been launched to generate the installers.");
        Debug.LogError(
            "Once installer generation completes, here are the remaining steps to take this new release live:\n"
            + "1) Upload the new /Releases installers to a new directory with today's date on Syn3h's CDN\n"
            + "2) Duplicate these releases, and overwrite the /latest CDN directory with them - https://cloud.google.com/storage/docs/gsutil/commands/cp\n"
            + ">   https://cloud.google.com/storage/docs/gsutil/commands/cp\n"
            + ">   gsutil cp gs://syn3h_releases/2017-06-16%20-%20Copy gs://syn3h_releases/latest\n"
            + "3) Update our CDN \"data\" file to inform clients of the new version\n"
        );
    }

    public static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        Debug.Log(outLine.Data);
    }

    public static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        Debug.LogError(outLine.Data);
    }

    // @deprecated - no one cares anymore
    // [MenuItem("Automation/Build Win 32", false, 20)]
    static void BuildWin32()
    {
        Debug.Log("BuildAutomation :: Building for Win32...");

        BuildPlayerOptions opts = GetDefaultBuildPlayerOptions();
        opts.target = BuildTarget.StandaloneWindows;
        opts.locationPathName += "_windows_32/" + GetAppName() + ".exe";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        Build(opts);
    }

    [MenuItem("Automation/Build Win", false, 20)]
    static void BuildWin()
    {
        Debug.Log("BuildAutomation :: Building for Win...");

        BuildPlayerOptions opts = GetDefaultBuildPlayerOptions();
        opts.target = BuildTarget.StandaloneWindows64;
        opts.locationPathName += "_windows/" + GetAppName() + ".exe";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        // note: in case of "Unhandled Exception: Unity.IL2CPP.Building.BuilderFailedException: Il2CppTypeDefinitions.cpp" "fatal error C1083: Cannot open include file: 'winapifamily.h': No such file or directory" errors,
        // install this: https://go.microsoft.com/fwlink/p/?LinkId=845298
//        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
        Build(opts);
    }

    [MenuItem("Automation/Build Mac", false, 20)]
    static void BuildMac()
    {
        Debug.Log("BuildAutomation :: Building for macOS...");

        BuildPlayerOptions opts = GetDefaultBuildPlayerOptions();
        opts.target = BuildTarget.StandaloneOSX;
        opts.locationPathName += "_macos/" + GetAppName() + ".app";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        Build(opts);
    }

    [MenuItem("Automation/Build Linux", false, 20)]
    static void BuildLinux()
    {
        Debug.Log("BuildAutomation :: Building for Linux...");

        BuildPlayerOptions opts = GetDefaultBuildPlayerOptions();
        opts.target = BuildTarget.StandaloneLinuxUniversal;
        opts.locationPathName += "_linux/" + GetAppName();
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        Build(opts);
    }

//    [MenuItem("Automation/Build Android")]
//    static void BuildAndroid()
//    {
//        BuildPlayerOptions opts = GetDefaultBuildPlayerOptions();
//        opts.target = BuildTarget.Android;
//        opts.locationPathName += "_Linux";
//        Build(opts);
//    }

//    [MenuItem("Automation/Build iOS")]
//    static void BuildIOS()
//    {
//        BuildPlayerOptions opts = GetDefaultBuildPlayerOptions();
//        opts.target = BuildTarget.iOS;
//        opts.locationPathName += "_iOS_Xcode";
//        Build(opts);
//    }

    private static string GetAppName()
    {
        return Application.productName;
    }

    private static string GetBuildDir()
    {
        return /*GetBuildVersion() + "_" + */GetAppName().ToLower();
    }

    private static string GetBuildVersion()
    {
        return Application.version;

        // @todo replace w/ CoreConfig-derived version
    }

    private static string[] GetBuildScenes()
    {
        return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
    }

    private static BuildPlayerOptions GetDefaultBuildPlayerOptions()
    {
        var opts = new BuildPlayerOptions
        {
            scenes = GetBuildScenes(),
            locationPathName = Application.dataPath + "/../Build/" + GetBuildDir(),
            options = BuildOptions.None
        };
        return opts;
    }

    static void Build(BuildPlayerOptions opts)
    {
        opts.locationPathName = Path.GetFullPath(opts.locationPathName);
        if (File.Exists(opts.locationPathName))
        {
            Debug.Log("BuildAutomation :: deleting preexisting build file :: " + opts.locationPathName);
            File.Delete(opts.locationPathName);
        }
        else if (Directory.Exists(opts.locationPathName))
        {
            Debug.Log("BuildAutomation :: deleting preexisting build dir :: " + opts.locationPathName);
            Directory.Delete(opts.locationPathName, true);
        }

        Debug.Log("BuildAutomation :: Building " + opts.target + " to " + opts.locationPathName);

        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log(
                "BuildAutomation :: Warning :: Build failed: \n"
                + "Result: \t" + report.summary.result + "\n"
                + "Path: \t" + opts.locationPathName + "\n"
                + "Error count: \t" + report.summary.totalErrors + "\n"
                + "Messages: \n" + (from o in report.steps select ">\t" + o.messages.ToString()) + "\n"
            );
        }
    }
}