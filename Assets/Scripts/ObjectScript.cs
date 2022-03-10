using System.Collections;
using System.Linq;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    // TODO : make restart and pause
    public string Id = "";
    public string[] Trajectory { get; set; }
    protected virtual int currLine { get; set; } = 0;

    protected virtual float alpha { get; } = 1;

    protected float step;
    private Rigidbody objRigidbody;
    protected bool stopped = false;
    protected float animDuration = 0.1f;
    protected float speed = 100f;

    public void Awake()
    {
        step = 100.0f * Time.deltaTime;
        objRigidbody = transform.GetComponent<Rigidbody>();
    }

    protected virtual void Stop()
    {
        objRigidbody.velocity = Vector3.zero;
        objRigidbody.isKinematic = true;
        stopped = true;
    }

    public IEnumerator ReadLine()
    {
        ++currLine;
        if (Trajectory.Length <= currLine)
        {
            Stop();
        }
        else if (Trajectory[currLine] != "still")
        {
            var commandData = Trajectory[currLine].Split('=');

            //yield return commandData[0] switch
            //{
            //    "pos" => StartCoroutine(Move(commandData[1])),
            //    "rot" => StartCoroutine(Rotate(commandData[1])),
            //    "beepState" => StartCoroutine(Beep(commandData[1])),
            //    "markerState" => StartCoroutine(Marker(commandData[1])),
            //    _ => StartCoroutine(ReadLine())
            //};
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
            }
            //yield return StartCoroutine(ReadLine());
            //yield return new WaitForSeconds(animDuration);
        }
    }

    protected IEnumerator Move(string data)
    {
        // calculate distance to move
        var coords = data.Split(' ').ToList().Select(x => float.Parse(x)).ToList();
        var target = new Vector3(coords[0], transform.position.y, alpha * coords[1]);
        //float t = 0;
        //while (t < 1)
        //{
        //    if (Vector3.Distance(transform.position, target) > 0.001)
        //    {
        //        transform.position = Vector3.Lerp(transform.position, target, t * speed);
        //    }
        //    t += Time.deltaTime / animDuration;
        //    yield return null;
        //}

        while (Vector3.Distance(transform.position, target) > 0.001)
        {
            transform.position = Vector3.Lerp(transform.position, target, step);
            yield return null;
        }
    }

    protected IEnumerator Rotate(string data)
    {
        // calculate distance to rotate
        var rotation = float.Parse(data) + 90f;
        var targetRotation = Quaternion.Euler(transform.rotation.x, rotation, transform.rotation.z);

        float t = 0;
        while (t < 1)
        {
            if (System.Math.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) > 0.01)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t * speed);
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
