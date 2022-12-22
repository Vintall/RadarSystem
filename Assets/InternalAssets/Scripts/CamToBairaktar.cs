using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToBairaktar : MonoBehaviour
{
    [SerializeField] Transform bairaktar;
    [SerializeField] Camera camera;
    [SerializeField, Range(0, 90)] float fov;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(bairaktar);

        if(Input.GetKey(KeyCode.Q))
        {
            camera.fieldOfView = fov;
        }
    }
}
