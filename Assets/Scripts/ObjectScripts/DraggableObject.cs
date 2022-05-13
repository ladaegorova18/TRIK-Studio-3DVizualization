using cakeslice;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
	// The plane the object is currently being dragged on
	protected Plane dragPlane;

	// The difference between where the mouse is on the drag plane and 
	// where the origin of the object is on the drag plane
	protected Vector3 offset;

	protected Camera mainCamera;
	public string Id;

	public delegate void SelectedChanged(DraggableObject selectedObject);
	public event SelectedChanged NotifySelectedChanged;

	private void Awake() => mainCamera = Camera.main;
	private bool scale;
	private bool rotate;
	private float rotationSpeed = 100.0f;

	public void UpdateColor(Color color)
	{
		var children = transform.GetComponentsInChildren<Renderer>();
		foreach (var childRenderer in children)
		{
			childRenderer.material.SetColor("_Color", color);
		}
	}

	public void ChangeTexture(Texture2D texture)
	{
		var children = transform.GetComponentsInChildren<Renderer>();
		foreach (var childRenderer in children)
		{
			childRenderer.material.mainTexture = texture;
		}
	}

	public Color GetColor() => transform.GetComponentInChildren<Renderer>().material.color;

	void OnMouseDown()
	{
		//NotifySelectedChanged(this);
		//SwitchOutline(false);

		//dragPlane = new Plane(mainCamera.transform.forward, transform.position);
		//var camRay = mainCamera.ScreenPointToRay(Input.mousePosition);

		//float planeDist;
		//dragPlane.Raycast(camRay, out planeDist);
		//offset = transform.position - camRay.GetPoint(planeDist);
	}

	void OnMouseDrag()
	{
		//if (rotate)
		//{
		//	Rotate();
		//} 
		//else if (scale)
		//{
		//	Resize();
		//} 
		//else
		//{
		//	Move();
		//}
	}

	public void SwitchOutline(bool value)
	{
		var children = transform.GetComponentsInChildren<Outline>();
		foreach (var childOutline in children)
		{
			childOutline.eraseRenderer = value;
		}
	}

	public void ChangeTranparency(bool value)
	{
		var children = transform.GetComponentsInChildren<Renderer>();
		foreach (var child in children)
		{
			child.enabled = value;
		}
	}

	public void ScalingOn() => scale = !scale;
	public void RotationOn() => rotate = !rotate;

	private void Move()
	{
		Ray camRay = mainCamera.ScreenPointToRay(Input.mousePosition);

		float planeDist;
		dragPlane.Raycast(camRay, out planeDist);
		var newPosition = camRay.GetPoint(planeDist) + offset;
		transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
	}

	private void Rotate()
	{
		float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
		float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;

		transform.Rotate(Vector3.up, -rotX);
		transform.Rotate(Vector3.right, rotY);
	}

	protected virtual void Resize()
	{
		float scaleValue = Input.GetAxis("Mouse Y");
		if (scaleValue > 0)
			transform.localScale = transform.localScale * 1.05f;
		else if (scaleValue < 0)
			transform.localScale = transform.localScale / 1.05f;
	}
}
