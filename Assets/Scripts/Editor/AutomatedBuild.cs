using UnityEditor;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class AutomatedBuild : EditorWindow
{
	[MenuItem("Build/Win64 %&b", false, 1)]
	public static void BuildWin64()
	{
		BuildSettingsWindow buildSettings = new BuildSettingsWindow();
		buildSettings.Init();
		buildSettings.Show();

		if (buildSettings.success)
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
			PlayerSettings.fullScreenMode = UnityEngine.FullScreenMode.Windowed;
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
	}

	[MenuItem("Build/OSX")]
	static void BuildOSX()
	{
		BuildSettingsWindow buildSettings = new BuildSettingsWindow();
		buildSettings.Init();
		buildSettings.Show();

		if (buildSettings.success)
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
			PlayerSettings.fullScreenMode = UnityEngine.FullScreenMode.Windowed;
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
	public bool success = false;
	
	private bool official = false;
	private string oldVersion = "1.0";
	private string newVersion = "0.0";

	[MenuItem("Example/Simple Recorder")]
	public void Init()
	{
		const int width = 400;
		const int height = 100;

		var x = (Screen.currentResolution.width - width) / 2;
		var y = (Screen.currentResolution.height - height) / 3f;

		GetWindow<BuildSettingsWindow>().position = new Rect(x, y, width, height);

		//TODO(Jitse): Get current file version
	}

	void OnGUI()
	{
		official = EditorGUILayout.Toggle("Official build", official);
		newVersion = EditorGUILayout.TextField("File version", newVersion);

		EditorGUILayout.Space();
		if (GUILayout.Button("Build"))
		{
			if (oldVersion.Equals(newVersion))
			{
				EditorGUILayout.LabelField("Please choose a new version.");
			}
			else
			{

			}
		}

		if (oldVersion.Equals(newVersion))
		{
			EditorGUILayout.LabelField("Please choose a new version.");
		}
		else
		{
			EditorGUILayout.LabelField("");
		}
	}
}