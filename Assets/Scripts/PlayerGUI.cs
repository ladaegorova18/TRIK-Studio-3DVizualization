using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{

    public void StartVizualize() => ConnectToTRIK.Connect();

    public void StopServer() => ConnectToTRIK.StopServer();

    //public void Pause()
    //{
    //    paused = !paused;
    //    if (paused)
    //    {
    //        StopCoroutine(StartProcess());
    //    }
    //    else
    //    {
    //        StartCoroutine(StartProcess());
    //    }
    //}

    //public void Reset()
    //{
    //    foreach (var item in objectsScripts)
    //    {
    //        item.Reset();
    //    }
    //}
}
