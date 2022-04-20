using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// Class for importing robot model on scene
/// </summary>
public class DrawStlMesh
{
	private string _fileName = "";
	private string _trianglescount = "";

	private int _total;//The number of triangles, _total*3 is the number of triangle vertices
	private int _number;
	private BinaryReader _binaryReader;

	private List<Vector3> _vertices;
	private List<int> _triangles;
	string fullPath = Directory.GetCurrentDirectory() + "\\Assets\\robot.stl";

	GameObject robot;

	/// <summary>
	///Get STL model name and triangle number
	/// </summary>
	private void GetFileNameAndTrianglesCount(string name)
	{
		fullPath = Directory.GetCurrentDirectory() + "\\Assets" + $"/{name}";
		using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
		{
			_fileName = Encoding.UTF8.GetString(br.ReadBytes(80));//In the stl binary file, the first 80 bytes are the model name
			_trianglescount = BitConverter.ToInt32(br.ReadBytes(4), 0).ToString();//The next 4 bytes store the number of model triangles
		}
	}
	/// <summary>
	/// Create STL model instance
	/// </summary>
	private GameObject CreateInstance()
	{
		int gameObjectCount = 60000;//Represents how many points are contained in an object (the number of vertices of a single Mesh in Unity is up to 65000)

		_total = int.Parse(_trianglescount);
		_number = 0;
		_binaryReader = new BinaryReader(File.Open(fullPath, FileMode.Open));

		//Discard the first 84 bytes
		_binaryReader.ReadBytes(84);

		_vertices = new List<Vector3>();//Store triangle vertex coordinates
		_triangles = new List<int>();//Store triangle index

		while (_number < _total)
		{
			byte[] bytes;
			//A group of 50 bytes, storing the normal vector of the triangle and the vertex data of the three points
			bytes = _binaryReader.ReadBytes(50);

			if (bytes.Length < 50)
			{
				_number += 1;
				continue;
			}
			//Only the vertex data of the triangle is used here, ignoring the vector
			Vector3 vec1 = new Vector3(BitConverter.ToSingle(bytes, 12), BitConverter.ToSingle(bytes, 16), BitConverter.ToSingle(bytes, 20));
			Vector3 vec2 = new Vector3(BitConverter.ToSingle(bytes, 24), BitConverter.ToSingle(bytes, 28), BitConverter.ToSingle(bytes, 32));
			Vector3 vec3 = new Vector3(BitConverter.ToSingle(bytes, 36), BitConverter.ToSingle(bytes, 40), BitConverter.ToSingle(bytes, 44));

			_vertices.Add(vec1);
			_vertices.Add(vec2);
			_vertices.Add(vec3);

			_number += 1;
		}

		//Write the index into the list, the value is 0 to the number of vertices in each object minus one
		for (int triNum = 0; triNum < _vertices.Count; triNum++)
		{
			int gameObhectIndex = triNum / gameObjectCount;//Mark which object is currently being assigned
			_triangles.Add(triNum - gameObhectIndex * gameObjectCount);
		}
		GameObject tem = null;

		for (int meshNumber = 0; meshNumber < _vertices.Count; meshNumber += gameObjectCount)
		{
			//Create GameObject
			tem = new GameObject(Path.GetFileNameWithoutExtension(fullPath));
			tem.name = meshNumber.ToString();
			MeshFilter mf = tem.AddComponent<MeshFilter>();
			MeshRenderer mr = tem.AddComponent<MeshRenderer>();

			Mesh m = new Mesh();
			mr.name = meshNumber.ToString();
			if ((_vertices.Count - meshNumber) >= gameObjectCount)
			{
				m.vertices = _vertices.ToArray().Skip(meshNumber).Take(gameObjectCount).ToArray();
				m.triangles = _triangles.ToArray().Skip(meshNumber).Take(gameObjectCount).ToArray();
			}
			else
			{
				m.vertices = _vertices.ToArray().Skip(meshNumber).Take(_vertices.Count - meshNumber).ToArray();
				m.triangles = _triangles.ToArray().Skip(meshNumber).Take(_vertices.Count - meshNumber).ToArray();
			}
			m.RecalculateNormals();

			mf.mesh = m;
			mr.material = new Material(Shader.Find("Standard"));

			_binaryReader.Close();

			tem.transform.SetParent(robot.transform);
			//Debug.Log(tem.name + ": number of vertices "+ _vertices.Count);
			tem.transform.localScale = tem.transform.localScale * 0.2f;
		}
		return tem;
	}

	private void Createbase()
	{
		robot = new GameObject("Robot");
		robot.transform.Rotate(new Vector3(0, -90, 0)); // rotate robot as in TRIK Studio

		var lineDrawer = new GameObject("LineDrawer");
		lineDrawer.transform.parent = robot.transform;
		lineDrawer.transform.localPosition = new Vector3(20, 4, 4);

		robot.tag = "robot";
		var robotObject = robot.AddComponent<RobotObject>();
		var serializedObject = new UnityEditor.SerializedObject(robotObject);
		serializedObject.FindProperty("Id").stringValue = "trikKitRobot";
		serializedObject.ApplyModifiedProperties();
		robotObject.lineMaterial = Resources.Load("Materials/Line_Material") as Material;

		var audio = robot.AddComponent<AudioSource>();
		audio.clip = Resources.Load("Sounds/beep") as AudioClip;

		var boxCollider = robot.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3(30, 12, 30);
		boxCollider.center = new Vector3(20, 11, 15);
	}

	private void CreateWheels()
	{
		GetFileNameAndTrianglesCount("wheel.stl");

		var wheel1 = CreateInstance();
		var wheel2 = CreateInstance();

		wheel1.transform.SetParent(robot.transform);
		wheel2.transform.SetParent(robot.transform);

		wheel1.transform.localPosition = new Vector3(-20, 0.4f, 1.3f);
		wheel2.transform.localPosition = new Vector3(20, 0.4f, 1.3f);
	}

	// Start is called before the first frame update
	public void CreateSTL()
	{
		Createbase();

		GetFileNameAndTrianglesCount("robot.stl");
		CreateInstance();

		CreateWheels();
	}
}
