using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float mainSpeed = 100.0f; //regular speed
    float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 1000.0f; //Maximum speed when holdin gshift
    private float totalRun = 1.0f;
    public float cam_sens_rotate; // скорость поворота
    public float cam_sens_move; // скорость движения
    public float cam_wheel;

    private float camX;
    private float camY;
    private float camZ;
    private Transform camTr;
    private GameObject cam;

    // Use this for initialization
    void Start()
    {
        cam = this.gameObject;
        //cam.transform.LookAt(Vector3.zero);
        camX = cam.transform.eulerAngles.x;
        camY = cam.transform.eulerAngles.y;
        camTr = cam.transform;
        camZ = Vector3.Distance(Vector3.zero, camTr.position);
    }

    // Update is called once per frame
    void Update()
    {
        camX = 0f;
        camY = 0f;
        camZ = 0f;
        // берем состояние мыши
        // а конкретно - именованных осей
        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");
        // и колесо мышки
        float mW = Input.GetAxis("Mouse ScrollWheel");

        // если нажата правая мышка
        if (Input.GetAxis("Fire2") > 0)
        {
			if (mX != 0)
			{
				camX += mX * cam_sens_rotate;
				camTr.transform.Rotate(Vector3.up, camX); // крутим оси "вверх"
			}
			if (mY != 0)
			{
				camY -= mY * cam_sens_rotate;
				camTr.transform.Rotate(Vector3.right, camY); // крутим по оси "вправо"
			}
		}
        // если крутили колесо мыши
        if (mW != 0)
        {
            camZ = mW * cam_wheel;
            // здесь интересно:
            // умножаем кватерион поворота камеры (угол поворота камеры)
            // на абсолютный вектор "вперед" или "назад"[с множителем от соответствующего шевеления колесом мышки]
            // и прибавляем результат к положению камеры
            camTr.transform.position += (Vector3)(camTr.transform.rotation * (camZ > 0 ? Vector3.forward * camZ : Vector3.back * (-camZ)));
        }
        //Mouse  camera angle done.  

        //Keyboard commands
        Vector3 p = GetBaseInput();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            p = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }

        p = p * Time.deltaTime;
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.Space))
        { //If player wants to move on X and Z axis only
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            transform.Translate(p);
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}
