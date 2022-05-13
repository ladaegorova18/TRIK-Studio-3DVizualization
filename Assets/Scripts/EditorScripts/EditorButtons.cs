using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using cakeslice;
using System.IO;

public class EditorButtons : MonoBehaviour
{
    // A list containing the available prefabs.
    [SerializeField]
    private List<GameObject> palette = new List<GameObject>();

    [SerializeField]
    private int paletteIndex;

    [SerializeField]
    private UnityEngine.UI.Toggle transparencyToggle;

    private Vector3 position = new Vector3(0, 15f, 0);

    private ObjectManager objManager;

	private void Awake()
	{
        objManager = GameObject.Find("Manager").GetComponent<ObjectManager>();
        transparencyToggle.onValueChanged.AddListener(OnToggleValueChanged);
	}

	private void OnToggleValueChanged(bool isOn) => objManager.ChangeTransparency(!isOn);

	public void AddRobotPressed()
    {
        var stl = new DrawStlMesh();
        stl.CreateSTL();
    }

    public void AddBallPressed()
    {
        var ball = Instantiate(palette[1], position, Quaternion.identity);
        objManager.AddDynamicObject(ball);
    }

    public void AddSkittlePressed()
    {
        var skittle = Instantiate(palette[0], position, Quaternion.identity);
        objManager.AddDynamicObject(skittle);
    }

    public void AddWallPressed()
    {
        var wall = Instantiate(palette[2], position, Quaternion.identity);
        objManager.AddDraggableObject(wall);
    }

	public void ExportScenePressed() => ExportScript.Export();

	public void AddObjectFromFilePressed()
    {
#if UNITY_EDITOR
        var path = EditorUtility.OpenFilePanel("Open object file", "", "fbx");
        if (path != null)
        {
            string[] separator = { "/Assets/" };
            path = "Assets/" + path.Split(separator, System.StringSplitOptions.None)[1];
            var objAsset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            var obj = Instantiate(objAsset, position, Quaternion.identity);

            var draggable = obj.AddComponent<DraggableObject>();
            obj.AddComponent<BoxCollider>();
			for (var i = 0; i < obj.transform.childCount; ++i)
			{
                //obj.transform.GetChild(i).gameObject.AddComponent<MeshRenderer>();
                obj.transform.GetChild(i).gameObject.AddComponent<cakeslice.Outline>();
            }
            draggable.SwitchOutline(true);
			obj.transform.localScale = new Vector3(100, 100, 100);
            objManager.AddDraggableObject(obj);
        }
#endif
    }

    public void ChangeTexturePressed()
    {
#if UNITY_EDITOR
        var path = EditorUtility.OpenFilePanel("Open Texture", "", "jpg");
        if (path != null)
        {
			var byteFile = File.ReadAllBytes(path);
            var texture = new Texture2D(256, 256);
            texture.LoadImage(byteFile);
            objManager.ChangeTexture(texture);
        }
#endif
    }
}
