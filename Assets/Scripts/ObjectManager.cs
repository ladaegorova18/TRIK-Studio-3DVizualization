using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private float speed = 1f;
    private bool paused = false;

    private Dictionary<string, ObjectScript> objectsDictionary = new Dictionary<string, ObjectScript>();
    private List<Deserializer.Frame> frames = new List<Deserializer.Frame>();
    private int currFrame = 0;

    /// <summary>
    /// Awake is called before the first frame update
    /// </summary>
    private void Awake()
    {
        var dynamicObjects = GameObject
            .FindGameObjectsWithTag("ball").ToList().Concat(GameObject
            .FindGameObjectsWithTag("skittle")).ToList().Concat(GameObject
            .FindGameObjectsWithTag("robot").ToList()); /// "balls" + "skittles" + "robot"

        foreach (var item in dynamicObjects)
        {
            var script = item.GetComponent(typeof(ObjectScript)) as ObjectScript;
            objectsDictionary.Add(script.Id, script);
        }
        StartCoroutine(PlayFrame());
    }

    /// <summary>
    /// Adds new frame to frame array
    /// </summary>
    public void AddFrame(string frameString)
    {
        string[] separator = { "{\n    \"frame\"" };
        var framesStrings = frameString.Split(separator, System.StringSplitOptions.None); 
        foreach (var str in framesStrings)
        {
            if (str != "")
            {
                frames.Add(Deserializer.ReadFrame(separator[0] + str));
            }
        }
    }

    /// TO DO: change
    /// when we read frames from file
    ///public void Play() => StartCoroutine(PlayFrameFrom());

    /// <summary>
    /// Plays one frame 
    /// </summary>
    public IEnumerator PlayFrame()
    {
        if (frames.Count > currFrame)
        {
            foreach (var objectState in frames[currFrame].frame)
            {
                StartCoroutine(objectsDictionary[objectState.id].ReadComplexLine(objectState.state));
            }
            ++currFrame;
        }

        yield return new WaitForSeconds(0.04f);
        yield return StartCoroutine(PlayFrame());
    }
}
