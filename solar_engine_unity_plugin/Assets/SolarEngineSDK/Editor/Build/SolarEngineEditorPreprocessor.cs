using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SolarEngine.Build
{
    public class SolarEngineEditorPreprocessor
    {
#if UNITY_2018_1_OR_NEWER
        public class AdjustEditorPreprocessor : IPreprocessBuildWithReport
#else
    public class AdjustEditorPreprocessor : IPreprocessBuild
#endif
        {
            public int callbackOrder { get; }
#if UNITY_2018_1_OR_NEWER
            public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
            {
                OnPreprocessBuild(report.summary.platform, string.Empty);
            }
#endif

            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                if (target == BuildTarget.Android)
                {
#if UNITY_ANDROID&&!SOLARENGINE_BYTEDANCE
                    RunPostProcessTasksAndroid();
                    CheckConfusion();
                    

#endif
                }else if (target == BuildTarget.iOS)
                {
                    PostProcessBuild_iOS(target, path);
                }
            }

            private static void PostProcessBuild_iOS(BuildTarget target, string buildPath)
            {
                
            }


            private const string SolorEngine = "[SolorEngine]";

            // [MenuItem("SolarEngineSDK/CheckConfusion ", false, 0)]
            public static void CheckConfusion()
            {
                if (PlayerSettings.Android.minifyRelease || PlayerSettings.Android.minifyDebug)
                {
                    Debug.Log(string.Format(SolorEngine)+" minifyRelease or minifyDebug is true");

                    var androidPluginsPath = Path.Combine(Application.dataPath, "Plugins/Android");
                    var appproguardPath = Path.Combine(Application.dataPath, "Plugins/Android/proguard-user.txt");
                    var seproguardPath= Path.Combine(Application.dataPath, "Plugins/SolarEngine/Android/proguard-user.txt");
                    
             

                    if (!File.Exists(appproguardPath))
                    {
                        if (!Directory.Exists(androidPluginsPath))
                        {
                            Directory.CreateDirectory(androidPluginsPath);
                            
                        }

                 
                        File.Copy(seproguardPath, appproguardPath);
                        Debug.Log( string.Format(SolorEngine)+" copy proguard-user.txt");
                    }
                    else
                    {
                        // 读取文件现有内容
                        string appContent = File.ReadAllText(appproguardPath);
                        string seContent = File.ReadAllText(seproguardPath);
                  
                        if (!appContent.Contains(seContent))
                        {
                         string updatedContent = appContent + seContent;
                         // 写入更新后的内容到文件
                         File.WriteAllText(appproguardPath, updatedContent);

                         Debug.Log(string.Format(SolorEngine)+$"Successfully added rule to keep  in proguard-user.txt  {seContent}");
                         
                        }else
                            {
                            Debug.Log(string.Format(SolorEngine)+$"already exists in proguard-user.txt   {seContent}  ");
                        }

                        
                    }
             }
            }
           // [MenuItem("SolarEngineSDK/RunPostProcessTasksAndroid ", false, 0)]
            
            public static void RunPostProcessTasksAndroid()
            {
                var androidPluginsPath = Path.Combine(Application.dataPath, "Plugins/Android");
                var seManifestPath =
                    Path.Combine(Application.dataPath, "Plugins/SolarEngine/Android/AndroidManifest.xml");
                var appManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
                var manifestHasChanged = false;
                if (!File.Exists(appManifestPath))
                {
                    if (!Directory.Exists(androidPluginsPath))
                    {
                        Directory.CreateDirectory(androidPluginsPath);
                    }

                 
                    File.Copy(seManifestPath, appManifestPath);

                }
                var manifestFile = new XmlDocument();
                manifestFile.Load(appManifestPath);
                
                manifestHasChanged |= AddURISchemes(manifestFile);
                if (manifestHasChanged)
                {
                    manifestFile.Save(appManifestPath);

                    Debug.Log(string.Format(SolorEngine)+"Successfully added URI schemes to AndroidManifest.xml");
                }
              
            }

            private static bool AddURISchemes(XmlDocument manifest)
            {
                if (SolarEngineSettings.AndroidUrlSchemes.Length == 0)
                    return false;
                
                var intentRoot = manifest.DocumentElement.SelectSingleNode("/manifest/application/activity[@android:name='com.unity3d.player.UnityPlayerActivity']", GetNamespaceManager(manifest));
                var usedIntentFiltersChanged = false;
                var usedIntentFilters = GetIntentFilter(manifest);
                
                foreach (var uriScheme in SolarEngineSettings.AndroidUrlSchemes)
                {
                    Uri uri;
                    try
                    {
                        // The first element is android:scheme and the second one is android:host.
                        uri = new Uri(uriScheme);

                        // Uri class converts implicit file paths to explicit file paths with the file:// scheme.
                        if (!uriScheme.StartsWith(uri.Scheme))
                        {
                            throw new UriFormatException();
                        }
                    }
                    catch (UriFormatException)
                    {
                        Debug.LogError(string.Format("[SolorEngine]: Android deeplink URI scheme \"{0}\" is invalid and will be ignored.", uriScheme));
                        Debug.LogWarning(string.Format("[SolorEngine]: Make sure that your URI scheme entry ends with ://"));
                        continue;
                    }
                    
                    if (!IsIntentFilterAlreadyExist(manifest, uri))
                    {
                        Debug.Log("[SolorEngine]: Adding new URI with scheme: " + uri.Scheme + ", and host: " + uri.Host);
                        var androidSchemeNode = manifest.CreateElement("data");
                        AddAndroidNamespaceAttribute(manifest, "scheme", uri.Scheme, androidSchemeNode);
                        AddAndroidNamespaceAttribute(manifest, "host", uri.Host, androidSchemeNode);
                        usedIntentFilters.AppendChild(androidSchemeNode);
                        usedIntentFiltersChanged = true;

                        Debug.Log(string.Format("[SolorEngine]: Android deeplink URI scheme \"{0}\" successfully added to your app's AndroidManifest.xml file.", uriScheme));
                    }
                }
                if (usedIntentFiltersChanged && usedIntentFilters.ParentNode == null)
                {
                    intentRoot.AppendChild(usedIntentFilters);
                }

                return usedIntentFiltersChanged;
            }

        }
        
        private static XmlElement GetIntentFilter(XmlDocument manifest)
        {
           // var xpath = "/manifest/application/activity/intent-filter[data/@android:scheme and data/@android:host]";
           var xpath = "/manifest/application/activity/intent-filter";
           
            var intentFilter = manifest.DocumentElement.SelectSingleNode(xpath, GetNamespaceManager(manifest)) as XmlElement;
      
            return intentFilter;
        }
        private static void AddAndroidNamespaceAttribute(XmlDocument manifest, string key, string value, XmlElement node)
        {
            var androidSchemeAttribute = manifest.CreateAttribute("android", key, "http://schemas.android.com/apk/res/android");
            androidSchemeAttribute.InnerText = value;
            node.SetAttributeNode(androidSchemeAttribute);
        }
        
        private static bool IsIntentFilterAlreadyExist(XmlDocument manifest, Uri link)
        {
            var xpath = string.Format("/manifest/application/activity/intent-filter/data[@android:scheme='{0}' and @android:host='{1}']", link.Scheme, link.Host);
            return manifest.DocumentElement.SelectSingleNode(xpath, GetNamespaceManager(manifest)) != null;
        }
        private static XmlNamespaceManager GetNamespaceManager(XmlDocument manifest)
        {
            var namespaceManager = new XmlNamespaceManager(manifest.NameTable);
            namespaceManager.AddNamespace("android", "http://schemas.android.com/apk/res/android");
            Debug.Log(namespaceManager);
            return namespaceManager;
        }
    }
}