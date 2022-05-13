//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ObjectTransform : MonoBehaviour
//{
//    private Vector3 lastMousePosition;
//    private DraggableObject gameObject;
//    private Camera camera;

//    // Start is called before the first frame update
//    void Start()
//    {
//        camera = Camera.main;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        var lastDistance = Vector3.Distance(camera.ScreenToWorldPoint(lastMousePosition), 
//            gameObject.transform.position);
//        var currDistance = Vector3.Distance(camera.ScreenToWorldPoint(Input.mousePosition),
//            gameObject.transform.position);
//        var sign = (lastDistance < currDistance) ? 1 : -1; /// cursor from object
//        gameObject.Resize(sign);

//        lastMousePosition = Input.mousePosition;
//    }

//    void Rotate()
//	{

//	}
//}
