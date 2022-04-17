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

	public void RunStopButtonPressed()
	{
		switch (playType)
		{
			case PlayType.FromFile:
				manager.RunPause();
				break;
			case PlayType.Realtime:
				ConnectionManager.RunStopButtonPressed();
				break;
		}
	}

	public void RestartPressed()
	{
		manager.ResetPositions();
		switch (playType)
		{
			case PlayType.FromFile:
				manager.Restart();
				break;
			case PlayType.Realtime:
				ConnectionManager.RestartPressed();
				break;
		}
	}
}
