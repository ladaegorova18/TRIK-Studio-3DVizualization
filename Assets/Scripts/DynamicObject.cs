using UnityEngine;

public class DynamicObject : ObjectScript
{
    //protected override int currLine { get; set; } = 1;

    /// <summary>
    /// On every collision starts new cycle of reading trajectory, if see "still", stops
    /// </summary>
    /// <param name="collision"> other object </param>
    //private void OnCollisionEnter(Collision collision)
    //{
    //    var colTag = collision.transform.tag; // add wheel collision
    //    if ((colTag == "ball" || colTag == "skittle" || colTag == "robot") && !stopped)
    //        StartCoroutine(ReadLine());
    //}
}
