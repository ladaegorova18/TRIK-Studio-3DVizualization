using System.Collections;
using System.Linq;
using UnityEngine;

public class RobotObject : ObjectScript
{
    public Material lineMaterial;

    private bool isDrawing = false;

    private Transform lineDrawer;
    private LineRenderer line;
    private AudioSource audioSource;

    private float countInterval = 0.25f;

    private Vector3 extents { get; set; }

    protected override float rotateAngle { get; } = 90f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lineDrawer = transform.Find("LineDrawer");
        extents = GetComponent<BoxCollider>().bounds.extents / 2;
    }

    public override void Reset()
    {
        base.Reset();
        var lines = GameObject.FindGameObjectsWithTag("line");
        foreach (var line in lines)
            Destroy(line);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawing)
        {
            /// If robot is drawing a line, add new point to it every 'countInterval' 
            if (countInterval >= 0.25f)
            {
                ++line.positionCount;
                line.SetPosition(line.positionCount - 1, lineDrawer.position);
                countInterval = 0;
            }
            countInterval += Time.deltaTime;
        }
    }

    protected override IEnumerator Move(string data)
    {
        // calculate distance to move
        var coords = data.Split(' ').ToList().Select(x => float.Parse(x)).ToList();
        var target = new Vector3(coords[0] - extents.x, transform.position.y, (-coords[1] - extents.z));
        //Debug.Log(target);
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

    /// <summary>
    /// Plays sound 
    /// </summary>
    /// <param name="data"> playing time in milliseconds </param>
    protected override IEnumerator Beep(string data)
    {
        var mSecs = int.Parse(data);
        int timeInterval = 250;
        while (mSecs > 0)
        {
            audioSource.Play();
            mSecs -= timeInterval;
        }
        yield return null;
    }

    /// <summary>
    /// Starts drawing new line or stops drawing current line
    /// </summary>
    /// <param name="data"> Line color (if transparent, line drawing stops)</param>
    protected override IEnumerator Marker(string data)
    {
        var markerState = data.Split(' ').ToList().Select(x => int.Parse(x)).ToList(); /// parse string rgb values to int
        var color = new Color(markerState[0], markerState[1], markerState[2], markerState[3]);

        if (color != Color.clear && (line == null || color != line.startColor))
        {
            isDrawing = true;
            var currentLine = new GameObject("Line");
            line = currentLine.AddComponent<LineRenderer>();
            line.positionCount = 1;
            line.material = lineMaterial;
            line.SetPosition(0, lineDrawer.position);
            line.startColor = line.endColor = color;
            line.tag = "line";
        }
        else isDrawing = false;
        yield return null;
    }
}
