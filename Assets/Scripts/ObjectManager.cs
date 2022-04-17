using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls actions of the robot and dynamic objects
/// </summary>
public class ObjectManager : MonoBehaviour
{
	private Dictionary<string, ObjectScript> objectsDictionary = new Dictionary<string, ObjectScript>();
	private List<Deserializer.Frame> frames = new List<Deserializer.Frame>();
	private int currFrame = 0;
	private bool paused = false;

	/// <summary>
	/// Awake is called before the first frame update
	/// </summary>
	private void Awake()
	{
		Deserializer.SetManager(this);
		ButtonScript.SetManager(this);

		var dynamicObjects = GameObject
			.FindGameObjectsWithTag("ball").ToList().Concat(GameObject
			.FindGameObjectsWithTag("skittle")).ToList().Concat(GameObject
			.FindGameObjectsWithTag("robot").ToList()); /// "balls" + "skittles" + "robot"

		foreach (var item in dynamicObjects)
		{
			var script = item.GetComponent(typeof(ObjectScript)) as ObjectScript;
			objectsDictionary.Add(script.Id, script);
		}
		StartCoroutine(Play());
	}

	/// <summary>
	/// Adds new frame to frame array
	/// </summary>
	public void AddFrame(Deserializer.Frame frame) => frames.Add(frame);

	/// <summary>
	/// Plays frames 
	/// </summary>
	public IEnumerator Play()
	{
		if (frames.Count > currFrame)
		{
			foreach (var objectState in frames[currFrame].frame)
			{
				if (objectsDictionary.ContainsKey(objectState.id))
					StartCoroutine(objectsDictionary[objectState.id].ReadComplexLine(objectState.state));
				else
				{
					Debug.Log(objectState.id + objectState.state + " !");
				}
			}
			++currFrame;
		}

		yield return new WaitForSeconds(0.04f); /// 24 frames per second => one frame lasts for ~0.04 sec 
		yield return StartCoroutine(Play());
	}

	public void RunPause()
	{
		paused = !paused;
		if (paused)
			StopCoroutine(Play());
		else
			StartCoroutine(Play());
	}

	public void ResetPositions()
	{
		foreach (var item in objectsDictionary.Values)
		{
			item.Reset();
		}
	}

	public void Restart()
	{
		currFrame = 0;
		Play();
	}

	/// <summary>
	/// To avoid stack overflow
	/// </summary>
	private void OnApplicationQuit() => ConnectionManager.StopServers();
}
