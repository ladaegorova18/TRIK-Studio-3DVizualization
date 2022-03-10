using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

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

    /// <summary>
    ///Get STL model name and triangle number
    /// </summary>
    private void GetFileNameAndTrianglesCount(string name)
    {
        fullPath = Directory.GetCurrentDirectory() + $"/{name}";
        using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
        {
            _fileName = Encoding.UTF8.GetString(br.ReadBytes(80));//In the stl binary file, the first 80 bytes are the model name
            _trianglescount = BitConverter.ToInt32(br.ReadBytes(4), 0).ToString();//The next 4 bytes store the number of model triangles
        }
    }
    /// <summary>
    /// Create STL model instance
    /// </summary>
    private void CreateInstance()
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

        for (int meshNumber = 0; meshNumber < _vertices.Count; meshNumber += gameObjectCount)
        {
            //Create GameObject
            GameObject tem = new GameObject(Path.GetFileNameWithoutExtension(fullPath));
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

            var parent = GameObject.Find("Robot");
            tem.transform.SetParent(parent.transform);
            //Debug.Log(tem.name + ": number of vertices "+ _vertices.Count);
            tem.transform.localScale = tem.transform.localScale * 0.2f;
        }
    }

    // Start is called before the first frame update
    public void CreateSTL()
    {
        GetFileNameAndTrianglesCount("Assets\\robot.stl");
        CreateInstance();

        GetFileNameAndTrianglesCount("Assets\\wheel.stl");
        CreateInstance();
    }
}
