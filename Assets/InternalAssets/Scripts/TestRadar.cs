using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRadar : MonoBehaviour
{
    [SerializeField] Transform tested_obj;
    const double LightSpeed = 300000000; // meters per second

    [SerializeField] Transform first_receiver;
    [SerializeField] Transform second_receiver;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    Vector3d? vec = null;
    Vector3d? pos = null;

    private void OnDrawGizmos()
    {
        if(tested_obj!= null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.2f);
            Gizmos.DrawSphere(tested_obj.position, Vector3.Distance(Camera.main.transform.position, tested_obj.position) / 50f);
        }
        if (pos != null && vec != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine((Vector3)pos.Value, (Vector3)pos.Value+(Vector3)vec.Value);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine((Vector3)pos.Value, ((Vector3)Vector3.right + (Vector3)pos.Value));

            Gizmos.color = Color.green;

            if (tested_obj != null)
                Gizmos.DrawLine((Vector3)pos.Value, tested_obj.position);
        }
    }
    public void CastRay()
    {
        double time_to_first_receiver;
        double time_to_second_receiver;
        double time_to_tracked_object;

        Vector3d first_receiver_position = new Vector3d(first_receiver.position.x, 0, first_receiver.position.z);
        Vector3d second_receiver_position = new Vector3d(second_receiver.position.x, 0, second_receiver.position.z);
        Vector3d radar_pos = new Vector3d(transform.position.x, 0, transform.position.z);
        {
            Vector3d tracked_obj_pos = new Vector3d(tested_obj.position.x, 0, tested_obj.position.z);
            double distance_to_obj = Vector3d.Distance(radar_pos, tracked_obj_pos);

            double distance_to_first_receiver = Vector3d.Distance(tracked_obj_pos, first_receiver_position);
            double distance_to_second_receiver = Vector3d.Distance(tracked_obj_pos, second_receiver_position);

            time_to_tracked_object = distance_to_obj / LightSpeed;
            time_to_first_receiver = distance_to_first_receiver / LightSpeed;
            time_to_second_receiver = distance_to_second_receiver / LightSpeed;
        }

        // Katet, that represent distance to second receiver, after it has touch first receiver.
        double delay_distance_between_receivers;
        double dist_between_receivers = Vector3d.Distance(first_receiver_position, second_receiver_position);
        double angle;

        Vector3d obj_pos;

        if (time_to_first_receiver < time_to_second_receiver)
        {
            delay_distance_between_receivers = LightSpeed * (time_to_second_receiver - time_to_first_receiver);

            angle = Mathd.Acos(delay_distance_between_receivers / dist_between_receivers) * Mathd.Rad2Deg;

            pos = first_receiver_position;
            vec = new Vector3d(Quaternion.AngleAxis((float)angle + 180, Vector3.up) * Vector3.right);

            obj_pos = LightSpeed * time_to_tracked_object * vec.Value + first_receiver_position;
        }
        else// if (time_to_first_receiver > time_to_second_receiver)
        {
            delay_distance_between_receivers = LightSpeed * (time_to_first_receiver - time_to_second_receiver);

            angle = -Mathd.Acos(delay_distance_between_receivers / dist_between_receivers) * Mathd.Rad2Deg;

            pos = second_receiver_position;
            vec = new Vector3d(Quaternion.AngleAxis((float)angle, Vector3.up) * Vector3.right);

            obj_pos = LightSpeed * time_to_tracked_object * vec.Value + second_receiver_position;
        }
        //else
        //{

        //}

        

        vec += (first_receiver_position + second_receiver_position) / 2;

        ClearLog();
        Debug.Log($"Angle: {Mathd.Round(angle)}");
        Debug.Log($"Impulse delay time: {time_to_tracked_object + time_to_first_receiver}");
        Debug.Log($"Time to tracked object: {time_to_tracked_object}");
        Debug.Log($"Time to echo to reach first receiver: {time_to_first_receiver}");
        Debug.Log($"Time to echo to reach second receiver: {time_to_second_receiver}");
        Debug.Log($"Real Obj Pos: {tested_obj.transform.position}");
        Debug.Log($"Found Obj Pos: {obj_pos}");
    }

    public void ClearLog()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
