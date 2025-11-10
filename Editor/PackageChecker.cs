#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public static class PackageChecker
{
    private static string packagePath = "";
    public static bool IsUPMPackageInstalled(string packageName="com.solarengine.sdk")
    {
        var listRequest = Client.List(true, false);
        while (!listRequest.IsCompleted) {} // 等待完成

        if (listRequest.Status == StatusCode.Success)
        {
            foreach (var pkg in listRequest.Result)
            {
                if (pkg.name == packageName)
                    packagePath = pkg.resolvedPath;
                    return true;
            }
        }
        return false;
    }

    public static string GetPackagePath()
    {
        return packagePath;
    }
    
}
#endif