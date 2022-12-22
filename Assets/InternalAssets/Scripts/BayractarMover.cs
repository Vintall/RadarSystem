using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BayractarMover : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody_;
    [SerializeField] Vector3 center;
    [SerializeField] Vector3 start_force;
    [SerializeField] float center_force;

    [SerializeField] Transform propeller;

    private void Start()
    {
        rigidbody_.AddForce(start_force);
    }
    Vector3 old_velocity;
    void Update()
    {
        Vector3 center_vector = (center - transform.position).normalized * center_force;
        float angle_to_center_vector = Vector3.SignedAngle(rigidbody_.velocity, center_vector, Vector3.up);

        rigidbody_.AddForce((center - transform.position).normalized * center_force * Time.deltaTime);
        transform.forward = rigidbody_.velocity;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -angle_to_center_vector / 5);


        propeller.Rotate(Vector3.right, -20, Space.Self);

    }
}
