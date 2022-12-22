using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BayraktarCamFolower : MonoBehaviour
{
    [SerializeField] Transform bayractar;
    Vector3 inital_pos;
    void Start()
    {
        executing = true;
        BakeInitialPos();
    }
    public bool executing = false;
    // Update is called once per frame
    void Update()
    {
        if (!executing)
            return;

        transform.position = inital_pos + bayractar.position;
    }
    public void BakeInitialPos()
    {
        inital_pos = transform.position - bayractar.position;
    }

}
