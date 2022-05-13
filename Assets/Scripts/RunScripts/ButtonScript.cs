using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Commands for GUI Buttons
/// </summary>
public class ButtonScript : MonoBehaviour
{
	private static PlayType playType;

	public Text errorText;
	public GameObject runButtons;
	public GameObject runOptions;

	enum PlayType { FromFile, Realtime }

	private static ObjectManager objManager;

	public static void SetManager(ObjectManager _manager) => objManager = _manager;

	public void RunFromFile()
	{
		try
		{
			runButtons.SetActive(false);
			runOptions.SetActive(true);
			objManager.Initialize();
			playType = PlayType.FromFile;
			Deserializer.AttachTrajectories("trajectory.json");
		}
		catch (System.Exception e)
		{
			errorText.text = e.Message;
		}
	}

	public void RunInRealtime()
	{
		try
		{
			runButtons.SetActive(false);
			runOptions.SetActive(true);
			objManager.Initialize();
			playType = PlayType.Realtime;
			ConnectionManager.Connect();
		}
		catch (System.Exception e)
		{
			errorText.text = e.Message;
		}
	}

	/// <summary>
	/// Handles run/stop button
	/// </summary>
	public void RunPausePressed()
	{
		try 
		{
			if (playType == PlayType.FromFile)
			{
				/// start/stop playing coroutine (thread)
				objManager.RunPausePressed();
			}
			else
			{
				/// send signal of run/pause
				ConnectionManager.RunPausePressed();
			}
		}
		catch (System.Exception e)
		{
			errorText.text = e.Message;
		}
	}

	/// <summary>
	/// Handles restart button
	/// </summary>
	public void RestartPressed()
	{
		try
		{ 
			switch (playType)
			{
				case PlayType.FromFile:
					/// set frame number to 0
					objManager.RestartFromFile();
					break;
				case PlayType.Realtime:
					/// send restart request to TRIK Studio
					ConnectionManager.RestartPressed();
					objManager.RestartRealTime();
					break;
			}
		}
		catch (System.Exception e)
		{
			errorText.text = e.Message;
		}
		///// set frame number to 0
		//objManager.Restart();
		//if (playType == PlayType.Realtime)
		//	/// send restart request to TRIK Studio
		//	ConnectionManager.RestartPressed();
	}
}
