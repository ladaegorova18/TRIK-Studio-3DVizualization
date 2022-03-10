using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class Deserializer : MonoBehaviour
{
    /// TODO: make it static

    List<ObjectScript> dynamicObjectsScripts = new List<ObjectScript>();
    ObjectScript robotScript;

    char[] charSeparator = { '\r' };

    /// <summary>
    /// Reads file from directory with application
    /// </summary>
    /// <param name="fileName"> Short file name with extention ("example.txt")</param>
    /// <returns> File content </returns>
    private string ReadFile(string fileName)
    {
        string commands;
        var pathToFile = $"{Directory.GetCurrentDirectory()}/{fileName}";
        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException();
        }

        using (var fstream = new FileStream(pathToFile, FileMode.Open))
        {
            var array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            commands = Encoding.Default.GetString(array);
        }
        return commands;
    }

    private string[] SplitByNewLines(string lines)
        => lines.Split(charSeparator, System.StringSplitOptions.RemoveEmptyEntries);

    private void DynamicTrajectories(string data)
    {
        /// trajectories for concrete objects
        string[] separator = { "newItem\r" };
        var dynamicObjectsTrajectories = data.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
        var trajDictionary = new Dictionary<string, string>();

        foreach (var itemTraj in dynamicObjectsTrajectories)
        {
            var idInfo = SplitByNewLines(itemTraj)[0].Split('=')[1]; /// we need x in "id=x\rpos=..."
            trajDictionary.Add(idInfo, itemTraj);
        }

        var dynamicObjects = GameObject.FindGameObjectsWithTag("ball").ToList().Concat(GameObject.FindGameObjectsWithTag("skittle")).ToList(); /// "balls" + "skittles"
        

        foreach (var item in dynamicObjects)
        {
            var objScript = item.GetComponent(typeof(ObjectScript)) as ObjectScript;
            objScript.Trajectory = (trajDictionary.ContainsKey(objScript.Id)) ? SplitByNewLines(trajDictionary[objScript.Id]) : new string[] { "" };
            dynamicObjectsScripts.Add(objScript);
        }
    }

    private void RobotTrajectories(string data)
    {
        robotScript = GameObject.FindGameObjectWithTag("robot").GetComponent<RobotObject>();
        string[] separator = { "newRobot\r" };
        var firstRobotTrajectory = data.Split(separator, System.StringSplitOptions.RemoveEmptyEntries)[0];

        robotScript.Trajectory = SplitByNewLines(firstRobotTrajectory); /// first part of commands is robot trajectory
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    public void ParseFile(string fileName)
    {
        string commands = ReadFile(fileName);

        string[] separator = { "robotPart\r" };
        var objectsTrajectories = commands.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

        RobotTrajectories(objectsTrajectories[0]);
        DynamicTrajectories(objectsTrajectories[1]);
    }

    public void StartVizualize() => StartCoroutine(StartProcess());

    private IEnumerator StartProcess()
    {
        foreach(var item in dynamicObjectsScripts)
        {
            StartCoroutine(item.ReadLine());
        }
        StartCoroutine(robotScript.ReadLine());

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(StartProcess());
    }
}
