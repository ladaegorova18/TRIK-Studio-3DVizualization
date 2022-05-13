using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SceneSerialization : MonoBehaviour
{
    public ObjectManager manager;
    private string pathToFile;

    [Serializable]
    public class RootObject
	{
        public SceneObject[] sceneObjects;
	}

    [Serializable]
	public class SceneObject
	{
        public string id;
        public string name;
        public float positionX;
        public float positionY;
        public float positionZ;
        public string color;

        public SceneObject (string _id, string _name, float _positionX, float _positionY, float _positionZ, string _color)
		{
            id = _id;
            name = _name;
            positionX = _positionX;
            positionY = _positionY;
            positionZ = _positionZ;
            color = _color;
		}
	}
    
    public void SaveScene()
	{
        var pathToFile = $"{Directory.GetCurrentDirectory()}/unityScene.json";
        var rootObject = new RootObject();
        var sceneObjectsList = new List<SceneObject>();
        foreach (var gameObject in manager.allObjects)
		{
            var pos = gameObject.transform.position;
            var sceneObj = new SceneObject(gameObject.GetComponent<DraggableObject>().Id,
                gameObject.name, pos.x, pos.y, pos.z, (gameObject.GetComponent<DraggableObject>().GetColor().ToString()));
            sceneObjectsList.Add(sceneObj);
		}
        rootObject.sceneObjects = sceneObjectsList.ToArray();
        var jsonFile = JsonUtility.ToJson(rootObject);
        using (var fstream = new FileStream(pathToFile, FileMode.OpenOrCreate))
        {
            byte[] buffer = Encoding.Default.GetBytes(jsonFile);
            fstream.Write(buffer, 0, buffer.Length);
        }
    }

    public void LoadScene()
    {
        var pathToFile = $"{Directory.GetCurrentDirectory()}/unityScene.json";
        string data;

        using (var fstream = new FileStream(pathToFile, FileMode.Open))
        {
            var array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            data = Encoding.Default.GetString(array);
        }

        var rootObject = JsonUtility.FromJson<RootObject>(data);

        for (var i = 0; i < rootObject.sceneObjects.Length; ++i)
		{

		}
    }

    public void ClearScene()
	{

	}
}
