/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015-Present Funplus
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using UnityEditor;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;


internal class ExportPackage {
		private static readonly string sdkVersion = SolarEngine.Analytics.sdk_version;

		private const string solarenginePath = "Assets/SolarEngineSDK/";

		private const string pluginsPath = "Assets/Plugins/";
		private const string editorPath = "Assets/Editor/";
		private const string resourcesPath = "Assets/Resources/";
	

		public enum Target
		{
			DEBUG,
			RELEASE
		}

		private static string PackageName
		{
			get
			{
				return "solarengine-unity-sdk.unitypackage";
			}
		}

		private static string outputPath
		{
			get
			{
				string projectRoot = Directory.GetCurrentDirectory();
				string outdir = Path.Combine (projectRoot, Path.Combine("release", "solarengine-unity-sdk-" + sdkVersion));
				if (Directory.Exists (outdir)) {
					Directory.Delete (outdir, true);
				}
					
				var outputDirectory = new DirectoryInfo(outdir);
				outputDirectory.Create();

				return Path.Combine(outputDirectory.FullName, PackageName);
			}
		}

		[MenuItem("SolarEngineInside/ExportPackage")]
		// Exporting the *.unityPackage for Asset store
		public static string exportPackage()
		{
			Debug.Log("Exporting SolarEngineSDK Unity Package...");
			string path = outputPath;
			
			BeforeExport ();
			
			try
			{
				string[] solarengineFiles = (string[])Directory.GetFiles(solarenginePath, "*.*", SearchOption.AllDirectories);
			
				string[] pluginsFiles = (string[])Directory.GetFiles(pluginsPath, "*.*", SearchOption.AllDirectories);
				string[] editorFiles = (string[])Directory.GetFiles(editorPath, "*.*", SearchOption.AllDirectories);
			
				string[] resourcesFiles = (string[])Directory.GetFiles(resourcesPath, "*.*", SearchOption.AllDirectories);
			
			
			string[] files = new string[solarengineFiles.Length + pluginsFiles.Length+editorFiles.Length+resourcesFiles.Length];
			
				solarengineFiles.CopyTo(files, 0);
				pluginsFiles.CopyTo(files, solarengineFiles.Length);
				editorFiles.CopyTo(files, solarengineFiles.Length + pluginsFiles.Length);
				resourcesFiles.CopyTo(files, solarengineFiles.Length + pluginsFiles.Length + editorFiles.Length);
				
			
				AssetDatabase.ExportPackage(
					files,
					path,
					ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);
			}
			finally
			{
				// Move files back no matter what
			}
			
			AfterExport ();
			
			Debug.Log("Exporting finished: " + Path.Combine("release", "solarengine-unity-sdk-" + sdkVersion));

			return path;
		}

		private static void BeforeExport()
		{
			string versionFile = Path.Combine (Directory.GetCurrentDirectory (), "VERSION");
			string versionStr = "Version: " + sdkVersion;
			File.WriteAllText (versionFile, versionStr);
		}

		private static void AfterExport()
		{
			string projectRoot = Directory.GetCurrentDirectory();
			string outdir = Path.Combine (projectRoot, Path.Combine("release", "solarengine-unity-sdk-" + sdkVersion));

			string srcPath = Directory.GetCurrentDirectory ();

			File.Copy (Path.Combine (srcPath, "VERSION"), Path.Combine(outdir, "VERSION"), true);
			string folderPath = outdir;

			// 在 macOS 中使用 "open" 命令打开文件夹
			try
			{
				Process.Start("open", folderPath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("无法打开文件夹：" + ex.Message);
			}
		}



		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}

