using System.Collections;

using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public Camera primaryCamera; //main camera
    //public Camera secondaryCamera;

    public float countDownValue = 5;
    public float countDown;

    public void Start()
    {
        primaryCamera.enabled = true;
        //secondaryCamera.enabled = false;

        countDown = countDownValue;
        //StartCoroutine(StartCountDown());
    }

    //public IEnumerator StartCountDown()
    //{
    //    while (countDown >= 0)
    //    {
    //        Debug.Log(countDown);
    //        yield return null;
    //        countDown -= Time.deltaTime;
    //    }

    //    if (countDown < 0)
    //    {
    //        primaryCamera.enabled = !primaryCamera.enabled;
    //        secondaryCamera.enabled = !secondaryCamera.enabled;
    //        countDown = 5;
    //    }
    //}

    //public void Update()
    //{
    //    if (UnityEngine.Input.GetKeyDown("x"))
    //    {
    //        primaryCamera.enabled = !primaryCamera.enabled;
    //        secondaryCamera.enabled = !secondaryCamera.enabled;
    //    }
    //}
}