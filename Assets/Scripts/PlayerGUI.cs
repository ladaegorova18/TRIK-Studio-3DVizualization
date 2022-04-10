using UnityEngine;

public class PlayerGUI : MonoBehaviour
{

    public void StartVizualize() => ConnectToTRIK.Connect();

    public void StopServer() => ConnectToTRIK.StopServers();

    public void RunStopButtonPressed() => ConnectToTRIK.RunStopButtonPressed();

    public void RestartPressed() => ConnectToTRIK.RestartPressed();

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
