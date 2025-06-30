
#if UNITY_IOS

using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace SolarEngine
{

    public class InstallSdkInXcode
	{

		private static readonly string[] SKIP_FILES = {@".*\.meta$"};

		private static string BuildPath { get; set; }

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				AfterBuild (pathToBuiltProject);
				RunPostBuildScript(target: buildTarget, projectPath: pathToBuiltProject);
			}
		}
		
#if TUANJIE_2022_3_OR_NEWER		
			// Must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and
			// that it's added before "pod install" (50).
			[PostProcessBuildAttribute(45)]
			private static void PostProcessBuild_iOS(BuildTarget target, string buildPath)
			{

				if (target == BuildTarget.iOS)
				{
		
					string podfilePath = Path.Combine(buildPath, "Podfile");
					try
					{
						string originalContent = File.ReadAllText(podfilePath, Encoding.UTF8);
						string modifiedContent = Regex.Replace(originalContent, "Unity-iPhone|UnityFramework", match =>
						{
							if (match.Value == "Unity-iPhone")
							{
								return "Tuanjie-iPhone";
							}
							else
							{
								return "TuanjieFramework";
							}
						});
						File.WriteAllText(podfilePath, modifiedContent);
					}
					catch (IOException e)
					{
						Debug.LogError($"SolarEngine {e.Message}");
					}
					
				}
				
			}
		#endif

		public static void AfterBuild (string buildPath)
		{
			BuildPath = buildPath;

			if (BuildPath == null)
			{
				return;
			}

		//	AddFrameworks();

            UnityEngine.Debug.Log ("Build success!");
		}

		private static void AddFrameworks ()
		{
#if TUANJIE_2022_3_OR_NEWER
			string projectFile = Path.Combine (BuildPath, "Tuanjie-iPhone.xcodeproj/project.pbxproj");
#else
			string projectFile = Path.Combine (BuildPath, "Unity-iPhone.xcodeproj/project.pbxproj");

#endif
			

			PBXProject pbxProject = new PBXProject ();
			pbxProject.ReadFromFile (projectFile);

			string target = pbxProject.TargetGuidByName ("UnityFramework");

			// ********** Frameworks ********** //
			pbxProject.AddFrameworkToProject (target, "AdServices.framework", false);
			pbxProject.AddFrameworkToProject (target, "iAd.framework", false);
			pbxProject.AddFrameworkToProject (target, "SystemConfiguration.framework", false);
			pbxProject.AddFrameworkToProject (target, "Security.framework", false);
			pbxProject.AddFrameworkToProject (target, "CoreTelephony.framework", false);
			pbxProject.AddFrameworkToProject (target, "AdSupport.framework", false);

			// pbxProject.AddFrameworkToProject (target, "AppTrackingTransparency.framework", false);

			// ********** Link Flags ********** //
			pbxProject.AddBuildProperty (target, "OTHER_LDFLAGS", "-ObjC -lz -lsqlite3 -licucore");
			pbxProject.WriteToFile (projectFile); 
		}


		private static void RunPostBuildScript(BuildTarget target, string projectPath = "")
		{
			if (target == BuildTarget.iOS)
			{
#if UNITY_IOS

				#if TUANJIE_2022_3_OR_NEWER
				string xcodeProjectPath = projectPath + "/Tuanjie-iPhone.xcodeproj/project.pbxproj";
#else
				string xcodeProjectPath = projectPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
#endif
			//	string xcodeProjectPath = projectPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

				PBXProject xcodeProject = new PBXProject();
				xcodeProject.ReadFromFile(xcodeProjectPath);

#if UNITY_2019_3_OR_NEWER
				string xcodeTarget = xcodeProject.GetUnityMainTargetGuid();
#else
            string xcodeTarget = xcodeProject.TargetGuidByName("Unity-iPhone");
#endif
				HandlePlistIosChanges(projectPath);

                if (SolarEngineSettings.iOSUniversalLinksDomains != null && SolarEngineSettings.iOSUniversalLinksDomains.Length > 0)
                {
                    AddUniversalLinkDomains(xcodeProject, xcodeProjectPath, xcodeTarget);
                }

				xcodeProject.WriteToFile(xcodeProjectPath);

			}
		}


		private static void HandlePlistIosChanges(string projectPath)
		{

			// Get and read info plist.
			var plistPath = Path.Combine(projectPath, "Info.plist");
			var plist = new PlistDocument();
			plist.ReadFromFile(plistPath);
			var plistRoot = plist.root;

			if (SolarEngineSettings.iOSUrlIdentifier != null && SolarEngineSettings.iOSUrlSchemes != null)
			{
				if (SolarEngineSettings.iOSUrlIdentifier.Length > 0 && SolarEngineSettings.iOSUrlSchemes.Length > 0)
				{
					AddUrlSchemesIOS(plistRoot, SolarEngineSettings.iOSUrlIdentifier, SolarEngineSettings.iOSUrlSchemes);
				}
			}
			// Write any info plist change.
			File.WriteAllText(plistPath, plist.WriteToString());
		}


		private static void AddUrlSchemesIOS(PlistElementDict plistRoot, string urlIdentifier, string[] urlSchemes)
  		{

			// ********** CFBundleURLTypes ********* //
			PlistElementArray bundleUrlTypes = null;
			if (plistRoot.values.ContainsKey("CFBundleURLTypes"))
			{
				bundleUrlTypes = plistRoot["CFBundleURLTypes"].AsArray();
			}
			else
			{
				bundleUrlTypes = plistRoot.CreateArray("CFBundleURLTypes");
			}

            foreach (var link in urlSchemes)
            {
				PlistElementDict bundleUrlSchemes = bundleUrlTypes.AddDict();
				PlistElementArray bundleUrlSchemesArray = null;

				bundleUrlSchemes.SetString("CFBundleURLName", urlIdentifier);
				bundleUrlSchemesArray = bundleUrlSchemes.CreateArray("CFBundleURLSchemes");
				bundleUrlSchemesArray.AddString(string.Format("{0}", link));
				 
			}
		}

		private static void AddUniversalLinkDomains(PBXProject project, string xCodeProjectPath, string xCodeTarget)
		{
			string entitlementsFileName = "Unity-iPhone.entitlements";

#if UNITY_2019_3_OR_NEWER
			var projectCapabilityManager = new ProjectCapabilityManager(xCodeProjectPath, entitlementsFileName, null, project.GetUnityMainTargetGuid());
#else
        var projectCapabilityManager = new ProjectCapabilityManager(xCodeProjectPath, entitlementsFileName, PBXProject.GetUnityTargetName());
#endif

			var uniqueDomains = SolarEngineSettings.iOSUniversalLinksDomains.Distinct().ToArray();
			const string applinksPrefix = "applinks:";
			for (int i = 0; i < uniqueDomains.Length; i++)
			{
				if (!uniqueDomains[i].StartsWith(applinksPrefix))
				{
					uniqueDomains[i] = applinksPrefix + uniqueDomains[i];
				}
			}

			projectCapabilityManager.AddAssociatedDomains(uniqueDomains);
			projectCapabilityManager.WriteToFile();

			project.AddCapability(xCodeTarget, PBXCapabilityType.AssociatedDomains, entitlementsFileName);
		}
#endif


		private static PlistElementArray CreatePlistElementArray(PlistElementDict root, string key)
		{
			if (!root.values.ContainsKey(key))
			{
				Debug.Log(string.Format("[SolarEngine]: {0} not found in Info.plist. Creating a new one.", key));
				return root.CreateArray(key);
			}
			var result = root.values[key].AsArray();
			return result != null ? result : root.CreateArray(key);
		}

		private static PlistElementDict CreatePlistElementDict(PlistElementArray rootArray)
		{
			if (rootArray.values.Count == 0)
			{
				Debug.Log("[SolarEngine]: Deeplinks array doesn't contain dictionary for deeplinks. Creating a new one.");
				return rootArray.AddDict();
			}

			var urlSchemesItems = rootArray.values[0].AsDict();
			Debug.Log("[SolarEngine]: Reading deeplinks array");
			if (urlSchemesItems == null)
			{
				Debug.Log("[SolarEngine]: Deeplinks array doesn't contain dictionary for deeplinks. Creating a new one.");
				urlSchemesItems = rootArray.AddDict();
			}

			return urlSchemesItems;
		}


	}




}

#endif


#if UNITY_STANDALONE_OSX

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;


namespace SolarEngine
{

	public class InstallSdkInXcode
	{



		
		[PostProcessBuild]
		public static void RunPostBuildScript(BuildTarget target, string pathToBuiltProject)
		{
			if (target == BuildTarget.StandaloneOSX)
			{
				string podfilePath = Path.Combine(pathToBuiltProject, "Podfile");

				// Step 1: pod init（如果没有 Podfile 才执行）
				if (!File.Exists(podfilePath))
				{
					Debug.Log("Podfile not found, running pod init...");
					ExecuteShellCommand("pod", "init", pathToBuiltProject);
				}
				else
				{
					Debug.Log("Podfile already exists, skipping pod init.");
				}

				// Step 2: 注入依赖（如果未注入）
				InjectPodsIntoPodfile(pathToBuiltProject);

				// Step 3: 执行 pod install
				ExecuteShellCommand("pod", "install", pathToBuiltProject);
			}
		}
		private static void ExecuteShellCommand(string command, string args, string workingDir)
		{
			ProcessStartInfo proc = new ProcessStartInfo
			{
				FileName = "/bin/bash",
				Arguments = $"-c \"export LANG=en_US.UTF-8; cd '{workingDir}'; {command} {args}\"",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (Process process = new Process { StartInfo = proc })
			{
				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();
				process.WaitForExit();

				Debug.Log($"Command output: {output}");
				if (!string.IsNullOrEmpty(error))
				{
					Debug.LogWarning($"Command error: {error}");
				}
			}
		}

		private static void InjectPodsIntoPodfile(string directory)
		{
			string podfilePath = Path.Combine(directory, "Podfile");
			if (!File.Exists(podfilePath))
			{
				Debug.LogError("Podfile does not exist at: " + podfilePath);
				return;
			}

			// 从 Unity 设置读取 macOS 最低系统版本（例如 "12.0"）
			string macOSVersion = PlayerSettings.macOS.targetOSVersion;
			if (string.IsNullOrEmpty(macOSVersion))
			{
				macOSVersion = "12.0"; // fallback
			}

			string platformLine = $"platform :ios, '{macOSVersion}'";

			var lines = File.ReadAllLines(podfilePath).ToList();
			var modifiedLines = new List<string>();
			bool injectedPods = false;
			bool platformSet = false;

			foreach (var line in lines)
			{
				string trimmed = line.Trim();

				// 替换或启用注释的 platform 行
				if (trimmed.StartsWith("platform"))
				{
					modifiedLines.Add(platformLine);
					platformSet = true;
					continue;
				}
				else if (!platformSet && trimmed.StartsWith("# platform"))
				{
					modifiedLines.Add(platformLine);
					platformSet = true;
					continue;
				}

				// 在 `end` 前插入 pod 依赖
				if (!injectedPods && trimmed == "end")
				{
					modifiedLines.Add("  pod 'SESDKRemoteConfig', '>=0'");
					modifiedLines.Add("  pod 'SolarEngineSDK', '>=0'");
					injectedPods = true;
				}

				modifiedLines.Add(line);
			}

			// 如果没有任何 platform 行，则在文件最顶部插入
			if (!platformSet)
			{
				modifiedLines.Insert(0, platformLine);
			}

			File.WriteAllLines(podfilePath, modifiedLines);
			Debug.Log($"Podfile updated with platform :macos, '{macOSVersion}' and pod dependencies.");
		}
	}
}




#endif





