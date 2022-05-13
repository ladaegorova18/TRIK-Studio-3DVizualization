using UnityEngine;

public class SwitchModes : MonoBehaviour
{
	[SerializeField]
	private GameObject playModeInterfce;

	[SerializeField]
	private GameObject editorInterface;

	public void EditorModePressed()
	{
		playModeInterfce.SetActive(false);
		editorInterface.SetActive(true);
	}

	public void PlayModePressed()
	{
		editorInterface.SetActive(false);
		playModeInterfce.SetActive(true);
	}
}
