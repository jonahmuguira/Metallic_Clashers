using System.Collections;

using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public Transform mainCamera;

    public Transform gameObjectOne;
    public Transform gameObjectTwo;

    public void Start()
    {
        mainCamera.position = new Vector3(0, 0, -10);

        gameObjectOne.position = new Vector3(-10, 0, 0);
        gameObjectTwo.position = new Vector3(10, 0, 0);
    }

    public void Update()
    {
        if (UnityEngine.Input.GetKeyDown("x"))
        {
            mainCamera.position = new Vector3(0, 0, -10);

            mainCamera.position = gameObjectOne.position;
        }

        if (UnityEngine.Input.GetKeyDown("c"))
        {
            mainCamera.position = new Vector3(0, 0, -10);

            mainCamera.position = gameObjectTwo.position;
        }

        if (UnityEngine.Input.GetKeyDown("v"))
        {
            mainCamera.position = new Vector3(0, 0, -10);
            mainCamera.position = mainCamera.position;
        }
    }
}