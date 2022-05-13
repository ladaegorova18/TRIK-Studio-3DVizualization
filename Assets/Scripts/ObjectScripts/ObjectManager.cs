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
	private bool pauseCalled = false;
	private bool restartCalled = false;
	private bool initialized = false;
	public GameObject objectMenu;
	public ColorPicker colorPicker;

	public List<GameObject> allObjects;
	private List<GameObject> dynamicObjects;

	private DraggableObject selectedObject;

	private Coroutine playCoroutine;

	public void Awake()
	{
		Deserializer.SetManager(this);
		ButtonScript.SetManager(this);

		dynamicObjects = GameObject
			.FindGameObjectsWithTag("ball").ToList().Concat(GameObject
			.FindGameObjectsWithTag("skittle")).ToList().Concat(GameObject
			.FindGameObjectsWithTag("robot").ToList()).ToList(); /// "balls" + "skittles" + "robot"

		allObjects = dynamicObjects.ToList().Concat(GameObject.FindGameObjectsWithTag("static")).ToList();
		foreach (var item in allObjects)
		{
			item.GetComponent<DraggableObject>().NotifySelectedChanged += ObjectSelected;
		}
		colorPicker.NotifyColorChanged += ChangeObjectColor;
	}

	/// ---------- Editor part ---------- ///
	
	void ChangeObjectColor(Color color)
	{
		selectedObject?.UpdateColor(color);
	}

	void ObjectSelected(DraggableObject selected)
	{
		objectMenu.SetActive(true);
		selectedObject?.SwitchOutline(true);
		selectedObject = selected;
	}

	public void ApplyChanges()
	{
		objectMenu.SetActive(false);
	}

	public void ChangeTexture(Texture2D texture)
	{
		selectedObject?.ChangeTexture(texture);
	}

	public void ChangeTransparency(bool value)
	{
		selectedObject?.ChangeTranparency(value);
	}

	public void Remove()
	{
		ApplyChanges();
		var toRemove = selectedObject.gameObject;
		allObjects.Remove(toRemove);
		if (dynamicObjects.Contains(toRemove))
			dynamicObjects.Remove(toRemove);
		Destroy(toRemove);
		selectedObject = null;
	}

	public void AddDraggableObject(GameObject draggableObject)
	{
		allObjects.Add(draggableObject);
		draggableObject.GetComponent<DraggableObject>().NotifySelectedChanged += ObjectSelected;
	}

	public void AddDynamicObject(GameObject dynamicObject)
	{
		AddDraggableObject(dynamicObject);
		dynamicObjects.Add(dynamicObject);
	}

	public void ScaleObjectPressed() => selectedObject.ScalingOn();
	public void RotatePressed() => selectedObject.RotationOn();

	/// ---------- Play Mode part ---------- /// 

	/// <summary>
	/// Awake is called before the first frame update
	/// </summary>
	public void Initialize()
	{
		if (!initialized)
		{
			ExportScript.Export();
			foreach (var item in dynamicObjects)
			{
				var script = item.GetComponent(typeof(ObjectScript)) as ObjectScript;
#if UNITY_EDITOR
				var serializedObject = new UnityEditor.SerializedObject(script);
				var id = serializedObject.FindProperty("Id");
				objectsDictionary.Add(id.stringValue, script);
#else
			objectsDictionary.Add(script.Id, script);
#endif
			}
			initialized = true;
			//playCoroutine = Play();
		}
		frames.Clear();
		playCoroutine = StartCoroutine(Play());
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
		if (restartCalled)
		{
			RestartRealTime();
			restartCalled = false;
		}
		else if (pauseCalled)
		{
			RunPausePressed();
			pauseCalled = false;
		}
		else if (frames.Count > currFrame)
		{
			foreach (var objectState in frames[currFrame].frame)
			{
				if (objectsDictionary.ContainsKey(objectState.id))
					StartCoroutine(objectsDictionary[objectState.id].ReadComplexLine(objectState.state));
			}
			++currFrame;
		}

		yield return new WaitForSeconds(0.04f); /// 24 frames per second => one frame lasts for ~0.04 sec 
		playCoroutine = StartCoroutine(Play());
		yield return playCoroutine;
	}

	public void PauseCall() => pauseCalled = true;
	public void RestartCall() => restartCalled = true;

	public void RunPausePressed()
	{
		paused = !paused;
		if (paused)
			StopCoroutine(playCoroutine);
		else
			playCoroutine = StartCoroutine(Play());
	}

	/// return all objects to start positions
	private void ResetPositions()
	{
		foreach (var item in objectsDictionary.Values)
			item.Reset();
		currFrame = 0;
	}

	public void RestartRealTime()
	{
		ResetPositions();
		frames.Clear();
	}

	public void RestartFromFile()
	{
		ResetPositions();
		Play();
	}

	///// return all objects to start positions
	//public void Restart()
	//{
	//	foreach (var item in objectsDictionary.Values)
	//		item.Reset();
	//	currFrame = 0;
	//}

	//public void RestartRealTime()
	//{
	//	frames.Clear();
	//}

	/// <summary>
	/// To avoid stack overflow
	/// </summary>
	private void OnApplicationQuit() => ConnectionManager.StopServers();
}
