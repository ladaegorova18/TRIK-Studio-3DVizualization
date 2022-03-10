using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour
{
    // keep a copy of the executing script
    private IEnumerator coroutine;

    // Use this for initialization
    void Start()
    {
        print("Starting " + Time.time);
        coroutine = WaitAndPrint(3.0f);
        StartCoroutine(coroutine);
        print("Done " + Time.time);
    }

    // print to the console every 3 seconds.
    // yield is causing WaitAndPrint to pause every 3 seconds
    public IEnumerator WaitAndPrint(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            print("WaitAndPrint " + Time.time);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StopCoroutine(coroutine);
            print("Stopped " + Time.time);
        }
    }
}