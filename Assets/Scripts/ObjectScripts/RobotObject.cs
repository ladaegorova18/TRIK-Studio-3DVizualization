using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Robot controlling class
/// </summary>
public class RobotObject : ObjectScript
{
	[SerializeField]
	public Material lineMaterial; /// line material is default, you can change it in inspector if necessary

	private bool isDrawing = false;
	private Transform lineDrawer; /// marker for next line point position 
	private LineRenderer line;
	private Color lineColor;
	private float lineWidth = 10;

	private AudioSource audioSource; /// sound to play, sets in inspector

	private float countInterval = 0.25f;

	/// <summary>
	/// robot position is rotated to 90 degrees 
	/// </summary>
	protected override float rotateAngle { get; } = 90f;

	// Start is called before the first frame update
	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		lineDrawer = transform.Find("LineDrawer");
		extents = GetComponent<BoxCollider>().size / 2;
		extents = new Vector3(extents.x - 30, extents.y, 30);
		audioSource.Play();
	}

	public override void Reset()
	{
		base.Reset();
		//isDrawing = false;
		var lines = GameObject.FindGameObjectsWithTag("line");
		foreach (var line in lines)
			Destroy(line);
		if (isDrawing)
			CreateLine(lineColor);
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
	/// <param name="data"> Line color (if it is transparent, line drawing stops)</param>
	protected override IEnumerator Marker(string data)
	{
		var markerState = data.Split(' ').ToList().Select(x => int.Parse(x)).ToList(); /// parse string rgb values to int
		var color = new Color(markerState[0], markerState[1], markerState[2], markerState[3]);

		if (color != Color.clear && (line == null || color != line.startColor))
		{
			isDrawing = true;
			lineColor = color;
			CreateLine(color);
		}
		else isDrawing = false;
		yield return null;
	}

	private void OnApplicationQuit() => isDrawing = false;

	private void CreateLine(Color color)
	{
		var currentLine = new GameObject("Line");
		line = currentLine.AddComponent<LineRenderer>();
		line.positionCount = 1;
		line.material = lineMaterial;
		line.SetPosition(0, lineDrawer.position);
		line.startColor = line.endColor = color;
		line.startWidth = line.endWidth = lineWidth;
		line.tag = "line";
	}
}
