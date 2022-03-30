using System.Collections;
using System.Linq;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    // TODO : make restart and pause
    public string Id = "";
    public string[] Trajectory { get; set; }
    protected virtual int currLine { get; set; } = 0;

    private Vector3 startPosition;
    private Quaternion startRotation;

    /// <summary>
    /// robot position is rotated to 90 degrees 
    /// </summary>
    protected virtual float rotateAngle { get; } = 0;

    protected float animDuration = 0.04f;
    protected float speed = 1f;

    public void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public virtual void Reset()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        currLine = 0;
    }

    public IEnumerator ReadLines(float count)
    {
        for (var i = 0; i < count; ++i)
        {
            yield return StartCoroutine(ReadLine(Trajectory[currLine]));
        }
    }

    public IEnumerator ReadComplexLine(string line)
    {
        var commands = line.Split('|');
        foreach (var command in commands)
        {
            yield return ReadLine(command);
        }
    }

    public IEnumerator ReadLine(string line)
    {
        //++currLine;
        //if (Trajectory.Length > currLine)
        //{
        var commandData = line.Split('=');

        switch (commandData[0])
            {
                case "pos":
                    {
                        yield return StartCoroutine(Move(commandData[1]));
                        break;
                    }
                case "rot":
                    {
                        yield return StartCoroutine(Rotate(commandData[1]));
                        break;
                    }
                case "beepState":
                    {
                        yield return StartCoroutine(Beep(commandData[1]));
                        break;
                    }
                case "markerState":
                    {
                        yield return StartCoroutine(Marker(commandData[1]));
                        break;
                    }
                default:
                    break;
            //}
        }
    }
    //yield return commandData[0] switch
    //{
    //    "pos" => StartCoroutine(Move(commandData[1])),
    //    "rot" => StartCoroutine(Rotate(commandData[1])),
    //    "beepState" => StartCoroutine(Beep(commandData[1])),
    //    "markerState" => StartCoroutine(Marker(commandData[1])),
    //    _ => StartCoroutine(ReadLine())
    //};

    protected virtual IEnumerator Move(string data)
    {
        // calculate distance to move
        var coords = data.Split(' ').ToList().Select(x => float.Parse(x)).ToList();
        var target = new Vector3(coords[0], transform.position.y, coords[1]);
        float t = 0;
        while (t < 1)
        {
            if (Vector3.Distance(transform.position, target) > 0.01)
            {
                transform.position = Vector3.Lerp(transform.position, target, t);
            }
            t += Time.deltaTime / animDuration;
            yield return null;
        }
    }

    protected IEnumerator Rotate(string data)
    {
        // calculate distance to rotate
        var rotation = float.Parse(data) + rotateAngle;
        var targetRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + rotation, transform.rotation.z);
        float t = 0;
        while (t < 1)
        {
            if (System.Math.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) > 0.01)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
            }
            t += Time.deltaTime / animDuration;
            yield return null;
        }
    }

    protected virtual IEnumerator Beep(string data)
    {
        yield return null;
    }

    protected virtual IEnumerator Marker(string data)
    {
        yield return null;
    }
}
