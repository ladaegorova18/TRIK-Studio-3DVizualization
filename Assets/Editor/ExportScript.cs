using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Linq;

public class ExportScript : EditorWindow
{
    [MenuItem("Window/TRIK Studio interface")]
    static void OpenWindow()
    {
        ExportScript window = (ExportScript)GetWindow(typeof(ExportScript));
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Export"))
        {
            Export();
        }
        if (GUILayout.Button("ImportSTL"))
        {
            var stl = new DrawStlMesh();
            stl.CreateSTL();
        }
    }

    /// <summary>
    /// Creates a tag in project, if it does not already exist
    /// This method was taken from https://bladecast.pro/unity-tutorial/create-tags-by-script
    /// </summary>
    public static void CreateTag(string tag)
    {
        var currAsset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");

        if (currAsset != null)
        { 
            var serializeObj = new SerializedObject(currAsset);
            var tags = serializeObj.FindProperty("tags");

            var tagsCount = tags.arraySize;

            // do not create duplicates
            for (int i = 0; i < tagsCount; i++)
            {
                var existingTag = tags.GetArrayElementAtIndex(i);
                if (existingTag.stringValue == tag) return;
            }

            tags.InsertArrayElementAtIndex(tagsCount);
            tags.GetArrayElementAtIndex(tagsCount).stringValue = tag;

            serializeObj.ApplyModifiedProperties();
            serializeObj.Update();
        }
    }

    /// <summary>
    /// Writes positions of "important" state objects to xml scene file for TRIK Studio
    /// </summary>
    /// <returns> Export status </returns>
    static string Export()
    {
        var tags = new string[] { "static", "ball", "skittle", "robot", "line" };
        foreach (var tag in tags) { CreateTag(tag); }

        var doc = XDocument.Load(Directory.GetCurrentDirectory() + "/template.xml");

        var root = doc.Root;
        if (root != null)
        {
            var world = root.Element("world");
            var robotElement = root.Element("robots").Element("robot");

            var staticObjects = GameObject.FindGameObjectsWithTag("static");
            var ballsObjects = GameObject.FindGameObjectsWithTag("ball");
            var skittlesObjects = GameObject.FindGameObjectsWithTag("skittle");
            var robot = GameObject.FindGameObjectWithTag("robot");

            if (robot == null)
            {
                return "There is no robot!";
            }

            Robot(robot, robotElement);
            Walls(staticObjects, world);
            BallsAndSkittles(ballsObjects, world, "ball");
            BallsAndSkittles(skittlesObjects, world, "skittle");
        }
        doc.Save("scene.xml");
        return "Successfully exported!";
    }

    /// <summary>
    /// Adds positions of dynamic objects (balls and skittles) to xml export file
    /// </summary>
    static XElement BallsAndSkittles(GameObject[] balls, XElement world, string name)
    {
        if (balls.Length > 0)
        {
            var i = 1;
            foreach (var ball in balls)
            {
                ball.GetComponent<DynamicObject>().Id = name + i.ToString();
                var position = ball.transform.position;
                var ballElement = new XElement(name, new XAttribute("id", name + i.ToString()),
                                                        new XAttribute("markerX", $"{position.x}"),
                                                        new XAttribute("markerY", $"{-position.z}"),
                                                        new XAttribute("x", $"{position.x}"),
                                                        new XAttribute("y", $"{-position.z}"));
                world.Element(name + "s").Add(ballElement);
                i++;
            }
        }
        return world;
    }

    /// <summary>
    /// Adds robot position and rotation to xml export file
    /// </summary>
    static XElement Robot(GameObject robot, XElement robotElement)
    {
        var robotBounds = robot.GetComponent<BoxCollider>().bounds;
        //Debug.Log("robotBounds.min.x " + robotBounds.min.x + " " + "robotBounds.min.z " + robotBounds.min.z);
        //Debug.Log("robotBounds.max.x " + robotBounds.max.x + " " + "robotBounds.max.z " + robotBounds.max.z);
        //Debug.Log("robotBounds.extents" + robotBounds.extents);
        //Debug.Log("robotBounds.size" + robotBounds.size);
        robotElement.Element("startPosition").Attribute("x").Value = $"{robotBounds.min.x}";
        robotElement.Element("startPosition").Attribute("y").Value = $"{-robotBounds.max.z}";

        robotElement.Attribute("position").Value = $"{robotBounds.min.x}:{-robotBounds.max.z}"; // upper left angle

        robotElement.Element("startPosition").Attribute("direction").Value = 
            robotElement.Attribute("direction").Value = $"{robot.transform.rotation.eulerAngles.y - 90}";

        return robotElement;
    }

    /// <summary>
    /// Adds static objects and walls positions to xml export file
    /// </summary>
    static XElement Walls(GameObject[] staticObjects, XElement world)
    {
        // (0, 200)      (100, 200)
        // (0,0)         (100, 0)

        // (0, 100)      (200, 100)
        // (0,0)         (200, 0)

        if (staticObjects.Length > 0)
        {
            var i = 1;
            foreach (var wall in staticObjects)
            {
                var bounds = wall.GetComponent<Renderer>().bounds;

                var begin = (x: bounds.min.x, z: 0.0);
                var end = (x: bounds.max.x, z: 0.0);
                var rotation = wall.transform.rotation.y;

                if (rotation > 0)
                {
                    begin.z = bounds.max.z;
                    end.z = bounds.min.z;
                }
                else
                {
                    begin.z = bounds.min.z;
                    end.z = bounds.max.z;
                }

                var wall1 = new XElement("wall", new XAttribute("id", "wall" + i.ToString()),
                                                 new XAttribute("begin", $"{begin.x}:{-begin.z}"),
                                                 new XAttribute("end", $"{end.x}:{-end.z}"));
                world.Element("walls").Add(wall1);
                i++;
            }
        }
        return world;
    }
}
