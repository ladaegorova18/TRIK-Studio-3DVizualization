using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

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

    [Serializable]
    public class State
    {
        public string id;
        public string state;
        public State (string _id, string _traj)
        {
            id = _id;
            state = _traj;
        }
    }

    [Serializable]
    public class Frame
        {
        public List<State> frame;
        }

    [Serializable]
    public class Frames
        {
        public List<Frame> frames;
    }

    public static Frame ReadFrame(string frame) => JsonUtility.FromJson<Frame>(frame);

    private Frames ReadJsonFile(string frames) => JsonUtility.FromJson<Frames>(frames);

        robotScript.Trajectory = SplitByNewLines(firstRobotTrajectory); /// first part of commands is robot trajectory
    }

    /// <summary>
    /// Parses file into separate trajectories
    /// </summary>
    public void AttachTrajectories(string fileName)
    {
        string commands = ReadFile(fileName);
        var frames = ReadJsonFile(commands);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(StartProcess());
    }
}
