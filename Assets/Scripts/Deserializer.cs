using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

/// <summary>
/// Handles operations of reading JSON files
/// </summary>
public class Deserializer
{
	private delegate void PauseButtonHandler();
	private static event PauseButtonHandler NotifyRunPauseButton;

	private delegate void RestartButtonHandler();
	private static event RestartButtonHandler NotifyRestartButton;

	private static ObjectManager manager;

	private enum Signals { Run, Stop, Restart }

	public static void SetManager(ObjectManager _manager)
	{
		manager = _manager;
		NotifyRunPauseButton += manager.PauseCall;
		NotifyRestartButton += manager.RestartCall;
	}

	/// <summary>
	/// Reads file from directory with application and parses it into separate frames
	/// </summary>
	/// <param name="fileName"> Short file name with extention ("example.txt")</param>
	/// <returns> File content </returns>
	private static Frames ReadFile(string fileName)
	{
		string commands;
		var pathToFile = $"{Directory.GetCurrentDirectory()}/Trajectories/{fileName}";

		if (!File.Exists(pathToFile))
		{
			throw new FileNotFoundException();
		}

		using (var fstream = new FileStream(pathToFile, FileMode.Open))
		{
			var array = new byte[fstream.Length];
			fstream.Read(array, 0, array.Length);
			commands = Encoding.Default.GetString(array);
		}

		return UnityEngine.JsonUtility.FromJson<Frames>(commands);
	}

	public static void ReadMessage(string message)
	{
		if (message != "")
		{
			if (message == "Stop" || message == "Run")
				NotifyRunPauseButton.Invoke();
			else if (message == "Restart")
				NotifyRestartButton.Invoke();
			else ParseFrameFromString(message);
		}
	}

	/// <summary>
	/// Adds new frame to frame array
	/// </summary>
	private static bool ParseFrameFromString(string frameString)
	{
		bool result = true;
		string[] separator = { "{\"frame\"" };
		var framesStrings = frameString.Split(separator, StringSplitOptions.None);
		foreach (var str in framesStrings)
		{
			if (str != "")
			{
				try
				{
					var frame = UnityEngine.JsonUtility.FromJson<Frame>(separator[0] + str);
					manager.AddFrame(frame);
				}
				catch (Exception e)
				{
					UnityEngine.Debug.Log(e.Message);
					result = false;
				}
			}
		}
		return result;
	}
	
	/// <summary>
	/// Read file and get all frames from it
	/// </summary>
	public static void AttachTrajectories(string fileName)
	{
		var frames = ReadFile(fileName);
		foreach (var frame in frames.frames)
		{
			manager.AddFrame(frame);
		}
	}

	/// <summary>
	/// State of one object
	/// </summary>
	[Serializable]
	public class State
	{
		public string id;
		public string state;

		public State(string _id, string _traj)
		{
			id = _id;
			state = _traj;
		}
	}

	/// <summary>
	/// Every frame contains a set of states
	/// </summary>
	[Serializable]
	public class Frame
	{
		public List<State> frame;
	}

	[Serializable]
	public class Frames
	{
		public List<Frame> frames;
	}
}
