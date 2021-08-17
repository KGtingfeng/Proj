using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class MyEditorScript
{
    static string outPath = "";
    static string STAGE_FIX = "外网";
    static string DEV_FIX = "内网";
    static bool isStage = false;

    [MenuItem("Tools/Build")]
    public static void Build()
    {
        SetPlayerSettings();
        SetParameters();

        string tempStr = DEV_FIX;
        if (isStage)
        {
            tempStr = STAGE_FIX;
        }
        //Debug.Log("-------------------------------------------------out put the APP EVN and Path: ",isStage,tempStr);
        DateTime dateTime = DateTime.Now;

        string apkName = string.Format("./release/android/{0}/{1}-{2}-{3}.apk", dateTime.Month.ToString().PadLeft(2, '0') + "-" + dateTime.Day, "TimeSand", tempStr, dateTime.Hour.ToString().PadLeft(2, '0') + ":" + dateTime.Minute);
        outPath = apkName;

        /*//outPath = Application.persistentDataPath + apkName;*/

        BuildPlayerOptions buildPlayerOptions = GetBuildPlayerOptions();

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("-------------------------------------------------Build succeeded: " + summary.totalSize + " bytes");
        }
        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("-------------------------------------------------Build failed");
        }
    }

    /// <summary>
    /// 根据jenkins的参数读取到buildsetting里
    /// </summary>
    /// <returns></returns>
    public static void SetParameters()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        foreach (string str in parameters)
        {
            Debug.Log("------------------------------------------------- " + str + "------------------------------------------------ ");
            if (str.StartsWith("ChooseServer"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    SetUrl(tempParam[1].Trim());
                    Debug.Log("IPConfig------------------------------------------------- " + IPConfig.IP_CONFIG + "------------------------------------------------ ");
                }
            }
        }
    }

    public static void SetUrl(string url)
    {
        string s = "public class IPConfig{\n public static string IP_CONFIG = \"@url\";\n}";
        s = s.Replace("@url", url);
        isStage = s.IndexOf("192") == -1;
        Debug.Log(s);
        string content = s.Trim();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
        string csFile = Application.dataPath + "/Scripts/Game/Net/IPConfig.cs";
        Debug.LogError("csFile: " + csFile);
        if (File.Exists(csFile))
        {
            byte[] readBytes = File.ReadAllBytes(csFile);
            string readVersion = System.Text.Encoding.UTF8.GetString(readBytes);
            Debug.LogError("[read] " + readVersion.Trim());
            Debug.LogError("[file:] delete");
            File.Delete(csFile);
        }
        FileStream fileStream = File.Create(csFile);
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();
    }


    public static void SetPlayerSettings()
    {

        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);
        PlayerSettings.productName = "时辰砂";
        PlayerSettings.Android.keystoreName = @"/Users/apple/Documents/Jenkins/loli.keystore";
        // one.keystore 密码
        PlayerSettings.Android.keystorePass = "wdsgame";
        // one.keystore 别名
        PlayerSettings.Android.keyaliasName = "loli";
        // 别名密码
        PlayerSettings.Android.keyaliasPass = "wdsgame";

        PlayerSettings.bundleVersion = "2.3.0";
        PlayerSettings.Android.bundleVersionCode = 100;

        PlayerSettings.applicationIdentifier = "com.guys.go.love.tea";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
    }


    public static BuildPlayerOptions GetBuildPlayerOptions()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = new[] { @"Assets\Scenes\BeginGame.unity" };
        buildPlayerOptions.locationPathName = outPath;
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
        buildPlayerOptions.options = BuildOptions.None;
        return buildPlayerOptions;

    }

}

