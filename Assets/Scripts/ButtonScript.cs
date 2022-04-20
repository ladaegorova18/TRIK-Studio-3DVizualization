using UnityEngine;

/// <summary>
/// Commands for GUI Buttons
/// </summary>
public class ButtonScript : MonoBehaviour
{
	private static PlayType playType;

	enum PlayType { FromFile, Realtime }

	private static ObjectManager manager;

	public static void SetManager(ObjectManager _manager) => manager = _manager;

	public void RunFromFile()
	{
		playType = PlayType.FromFile;
		Deserializer.AttachTrajectories("trajectory1.json");
	}

	public void RunInRealtime()
	{
		playType = PlayType.Realtime;
		ConnectionManager.Connect();
	}

	/// <summary>
	/// Handles run/stop button
	/// </summary>
	public void RunPauseButtonPressed()
	{
		switch (playType)
		{
			case PlayType.FromFile:
				/// start/stop playing coroutine (thread)
				manager.RunPause();
				break;
			case PlayType.Realtime:
				/// send signal of run/pause
				ConnectionManager.RunPauseButtonPressed();
				break;
		}
	}

	/// <summary>
	/// Handles restart button
	/// </summary>
	public void RestartPressed()
	{
		switch (playType)
		{
			case PlayType.FromFile:
				/// set frame number to 0
				manager.RestartFromFile();
				break;
			case PlayType.Realtime:
				/// send restart request to TRIK Studio
				ConnectionManager.RestartPressed();
				manager.RestartRealTime();
				break;
		}
	}
}
