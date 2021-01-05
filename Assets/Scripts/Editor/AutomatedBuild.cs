using UnityEditor;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Linq;

public class AutomatedBuild : EditorWindow
{
	[MenuItem("Build/Win64 %&b", false, 1)]
	public static void BuildWin64()
	{
		BuildSettingsWindow buildSettings = new BuildSettingsWindow();
		buildSettings.Init(OS.Win64);
	}

	[MenuItem("Build/OSX")]
	public static void BuildOSX()
	{
		BuildSettingsWindow buildSettings = new BuildSettingsWindow();
		buildSettings.Init(OS.OSX);
	}

	public static void StartBuildOSX()
	{
		string branch = GetBranch();
		BuildPlayerOptions options;

		string path = "builds/" + branch + "/EditorOSX/";
		options = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Editor.unity" },
			locationPathName = path + "VivistaEditor.exe",
			target = BuildTarget.StandaloneOSX,
			options = 0
		};
		PlayerSettings.virtualRealitySupported = false;
		PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
		PlayerSettings.defaultIsNativeResolution = true;
		PlayerSettings.usePlayerLog = true;
		PlayerSettings.resizableWindow = true;
		BuildPipeline.BuildPlayer(options);

		path = "builds/" + branch + "/PlayerOSX/";
		options = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Player.unity" },
			locationPathName = path + "VivistaPlayer.exe",
			target = BuildTarget.StandaloneOSX,
			options = 0
		};
		PlayerSettings.virtualRealitySupported = false;
		BuildPipeline.BuildPlayer(options);

		ShowInWindowsExplorer("builds/" + branch);
	}

	public static void StartBuildWin64()
	{
		string branch = GetBranch();
		BuildPlayerOptions options;

		string path = "builds/" + branch + "/EditorWin/";
		options = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Editor.unity" },
			locationPathName = path + "VivistaEditor.exe",
			target = BuildTarget.StandaloneWindows64,
			options = 0
		};
		PlayerSettings.virtualRealitySupported = false;
		PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
		PlayerSettings.defaultIsNativeResolution = true;
		PlayerSettings.usePlayerLog = true;
		PlayerSettings.resizableWindow = true;
		BuildPipeline.BuildPlayer(options);

		path = "builds/" + branch + "/PlayerWin/";
		options = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Player.unity" },
			locationPathName = path + "VivistaPlayer.exe",
			target = BuildTarget.StandaloneWindows64,
			options = 0
		};
		PlayerSettings.virtualRealitySupported = true;
		BuildPipeline.BuildPlayer(options);

		ShowInWindowsExplorer("builds/" + branch);
	}

	public static string GetBranch()
	{
		var proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "git",
				Arguments = "rev-parse --abbrev-ref HEAD",
				RedirectStandardOutput = true,
				UseShellExecute = false
			}
		};
		proc.Start();
		return proc.StandardOutput.ReadToEnd().Trim();
	}

	public static void ShowInWindowsExplorer(string folder)
	{
		var cleaned = Path.GetFullPath(folder);
		UnityEngine.Debug.Log(cleaned);
		var proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "explorer.exe",
				Arguments = $"/select,\"{cleaned}\"",
			}
		};
		proc.Start();
	}
}

public class BuildSettingsWindow : EditorWindow
{
	private bool official = false;
	private bool beta = false;
	private OS operatingSystem;
	private string oldVersion = "1.0";
	private string newVersion = "";
	private string[] existingTags;

	[MenuItem("Build Settings")]
	public void Init(OS operatingSystem)
	{
		this.operatingSystem = operatingSystem;

		const int width = 450;
		const int height = 150;

		var x = (Screen.currentResolution.width - width) / 2;
		var y = (Screen.currentResolution.height - height) / 3;

		EditorWindow window = GetWindow<BuildSettingsWindow>(title:"Build Settings");
		window.position = new Rect(x, y, width, height);
		GUIContent title = new GUIContent();
		title.text = "Build Settings";
		window.titleContent = title;
		window.Show();

		oldVersion = GetLatestBuildVersion();
		existingTags = GetExistingTags();
	}

	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		official = EditorGUILayout.Toggle("Official build", official);
		EditorGUILayout.LabelField("Warning: this will create a new git tag.");
		EditorGUILayout.EndHorizontal();

		if (official)
		{
			beta = EditorGUILayout.Toggle("Beta", beta);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField($"Current version\t\t   {oldVersion}");
			newVersion = EditorGUILayout.TextField("New version", newVersion);

			EditorGUILayout.Space();
		}

		if (GUILayout.Button("Build"))
		{
			//NOTE(Jitse): Only official builds require a new tag to be made
			if (official)
			{
				//NOTE(Jitse): If the build version tag doesn't exist yet, create the new tag and start building
				if (!existingTags.Contains(newVersion))
				{
					string args = $"tag v{newVersion}";
					if (beta)
					{
						args = args + "-beta";
					}
					var proc = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = "git",
							Arguments = args,
							RedirectStandardOutput = true,
							UseShellExecute = false
						}
					};
					proc.Start();

					File.WriteAllText(@"Assets\Resources\BuildVersion.txt", newVersion);

					StartBuild();
				}
			}
			else
			{
				StartBuild();
			}
		}

		EditorGUILayout.BeginHorizontal();
		if (existingTags.Contains(newVersion) && official)
		{
			EditorGUILayout.LabelField("This version already exists.");
		}
		else
		{
			EditorGUILayout.LabelField("");
		}

		if (official && newVersion.Length > 0)
		{
			string versionPreview = $"Tag preview: v{newVersion}";
			if (beta)
			{
				versionPreview = versionPreview + "-beta";
			}
			EditorGUILayout.LabelField(versionPreview);
		}
		
		EditorGUILayout.EndHorizontal();

	}

	private void StartBuild()
	{
		switch (operatingSystem)
		{
			case OS.Win64:
				AutomatedBuild.StartBuildWin64();
				break;
			case OS.OSX:
				AutomatedBuild.StartBuildOSX();
				break;
		}

		Close();
	}

	public static string GetLatestBuildVersion()
	{
		var proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "git",
				Arguments = "rev-list --tags --max-count=1",
				RedirectStandardOutput = true,
				UseShellExecute = false
			}
		};
		proc.Start();
		string commitId = proc.StandardOutput.ReadToEnd().Trim('\n');

		proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "git",
				Arguments = $"describe --tags {commitId}",
				RedirectStandardOutput = true,
				UseShellExecute = false
			}
		};
		proc.Start();
		string version = proc.StandardOutput.ReadToEnd().Trim('\n');

		//NOTE(Jitse): Remove any prefixes and affixes
		return FormatVersionNumber(ref version);
	}

	private static string FormatVersionNumber(ref string version)
	{
		bool isNumber = int.TryParse(version[0].ToString(), out _);
		while (!isNumber)
		{
			version = version.Substring(1);
			isNumber = int.TryParse(version[0].ToString(), out _);
		}

		bool isNumberOrDot;
		for (int i = 0; i < version.Length; i++)
		{
			isNumberOrDot = int.TryParse(version[i].ToString(), out _) || version[i].Equals('.');
			if (!isNumberOrDot)
			{
				version = version.Substring(0, i);
				break;
			}
		}

		return version;
	}

	public static string[] GetExistingTags()
	{
		var proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "git",
				Arguments = "tag",
				RedirectStandardOutput = true,
				UseShellExecute = false
			}
		};
		proc.Start();
		string[] tags = proc.StandardOutput.ReadToEnd().Split('\n');

		for (int i = 0; i < tags.Length; i++)
		{
			string tag = tags[i];
			if (!string.IsNullOrEmpty(tag))
			{
				tags[i] = FormatVersionNumber(ref tag);
			}
		}

		return tags;
	}
}

public enum OS
{
	Win64,
	OSX
}