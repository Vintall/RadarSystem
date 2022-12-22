using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class RadarScutter : MonoBehaviour
{
    [SerializeField] float beamwidth_angle;
    [SerializeField] float max_distance;
    [SerializeField] double wave_length;

    [SerializeField] ScutterRayMeshBuilder scutter_ray_mesh_builder;
    [SerializeField] ScutterCollider scutter_collider;

    [SerializeField] List<Antenna> antennas = new List<Antenna>();
    [SerializeField] Transform antennas_holder;

    [SerializeField] GameObject antenna_prefab;

    [SerializeField] Material radar_image_plane_material;

    List<(List<(Vector4, double)>, GameObject)> tracked_points = new List<(List<(Vector4, double)>, GameObject)>();

    public List<(List<(Vector4, double)>, GameObject)> TrackedPoints => tracked_points;

    static RadarScutter instance;
    public static RadarScutter Instance => instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        if (tracked_points == null)
            tracked_points = new List<(List<(Vector4, double)>, GameObject)>();

        tracked_points.Clear();

        SetElements();
    }
    public void SetElements()
    {
        List<Vector4> only_points = new List<Vector4>();

        

        for (int i = 0; i < tracked_points.Count; i++)
            only_points.Add(new Vector2(0.5f - (float)(tracked_points[i].Item1[tracked_points[i].Item1.Count - 1].Item1.x / max_distance / 2), 1.0f - (float)(tracked_points[i].Item1[tracked_points[i].Item1.Count - 1].Item1.y / max_distance / 2)));

        if (only_points.Count < 500)
            while (only_points.Count < 500)
                only_points.Add(Vector2.zero);

        radar_image_plane_material.SetVectorArray("points_array", only_points);
        radar_image_plane_material.SetInt("points_count", tracked_points.Count);
    }

    float current_angle = 0;
    const double LightSpeed = 300000000; // Meters per second


    public void GenerateAntennas(int count, float distance)
    {
        if (antennas == null)
            antennas = new List<Antenna>();

        for (int i = 0; i < antennas.Count; i++)
            Destroy(antennas[i].gameObject);

        antennas.Clear();


        for (int i = 0; i < count; i++)
        {
            Antenna buff = Instantiate(antenna_prefab, antennas_holder).GetComponent<Antenna>();
            buff.transform.localPosition = new Vector3(distance * (1 + 2 * i - count) / 2, 0, 0);

            antennas.Add(buff);
        }
    }
    public void OnScutterColliderHit(Collider other)
    {
        Debug.Log("OnScutterColliderHitCallback");
        ScutterRayToObject(other);
    }
    void ScutterRayToObject(Collider hit_collider)
    {
        double time_to_first_receiver;
        double time_to_second_receiver;
        double time_to_tracked_object;
        double doppler_frequency;
        double base_frequency = LightSpeed / wave_length;

        Vector3d pos;
        Vector3d vec;

        Vector3d first_receiver_position = new Vector3d(antennas[0].transform.position.x, 0, antennas[0].transform.position.z);
        Vector3d second_receiver_position = new Vector3d(antennas[antennas.Count - 1].transform.position.x, 0, antennas[antennas.Count - 1].transform.position.z);
        Vector3d radar_pos = new Vector3d(transform.position.x, 0, transform.position.z);

        { //Variables, defined in this local space are forbidden to use in position calculations.
            Vector3d tracked_obj_pos = new Vector3d(hit_collider.transform.position.x, 0, hit_collider.transform.position.z);
            double distance_to_obj = Vector3d.Distance(radar_pos, tracked_obj_pos);

            double distance_to_first_receiver = Vector3d.Distance(tracked_obj_pos, first_receiver_position);
            double distance_to_second_receiver = Vector3d.Distance(tracked_obj_pos, second_receiver_position);

            //Need to be signed
            Vector3 speed_projection = Vector3.Project(hit_collider.GetComponent<Rigidbody>().velocity, transform.position - hit_collider.transform.position);

            float radial_speed = Vector3.Magnitude(speed_projection);

            float speed_vec_angle = Vector3.Angle(hit_collider.transform.position, speed_projection);
            if (speed_vec_angle < 90)
                radial_speed *= -1;

            doppler_frequency = base_frequency * (LightSpeed / (LightSpeed + radial_speed));

            //ClearLog();
            //Debug.Log($"Velocity vec: {hit_collider.GetComponent<Rigidbody>().velocity}");
            //Debug.Log($"RedialSpeed: {radial_speed}");
            //Debug.Log($"Base freq: {base_frequency}");
            //Debug.Log($"Doppler freq: {doppler_frequency}");
            //Debug.Log($"Doppler shift: {base_frequency - doppler_frequency}");

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

            obj_pos = LightSpeed * time_to_tracked_object * vec + first_receiver_position;
        }
        else// if (time_to_first_receiver > time_to_second_receiver)
        {
            delay_distance_between_receivers = LightSpeed * (time_to_first_receiver - time_to_second_receiver);

            angle = -Mathd.Acos(delay_distance_between_receivers / dist_between_receivers) * Mathd.Rad2Deg;

            pos = second_receiver_position;
            vec = new Vector3d(Quaternion.AngleAxis((float)angle, Vector3.up) * Vector3.right);

            obj_pos = LightSpeed * time_to_tracked_object * vec + second_receiver_position;
        }
        vec += (first_receiver_position + second_receiver_position) / 2;


        bool is_has_obj = false;
        int obj_index = -1;
        for (int i = 0; i < tracked_points.Count; i++)
        {
            if (tracked_points[i].Item2 == hit_collider.gameObject)
            {
                is_has_obj = true;
                obj_index = i;
                break;
            }    
        }

        double founded_radial_speed = LightSpeed * base_frequency / doppler_frequency - LightSpeed;

        if (!is_has_obj)
        {
            tracked_points.Add((new List<(Vector4, double)>(), hit_collider.gameObject));
            tracked_points[tracked_points.Count - 1].Item1.Add((new Vector2((float)obj_pos.x, (float)obj_pos.z), Time.timeAsDouble));

            UIController.Instance.AddNode(tracked_points.Count - 1, new Vector2((float)obj_pos.x, (float)obj_pos.z), founded_radial_speed);
        }
        else
        {
            tracked_points[obj_index].Item1.Add((new Vector2((float)obj_pos.x, (float)obj_pos.z), Time.timeAsDouble));
            UIController.Instance.ChangeNode(obj_index, new Vector2((float)obj_pos.x, (float)obj_pos.z), founded_radial_speed);
        }
        SetElements();

        ClearLog();

        Debug.Log(new Vector2(0.5f - (float)(obj_pos.x / max_distance / 2), 1.0f - (float)(obj_pos.z / max_distance / 2)));
        Debug.Log($"Points count: {tracked_points.Count}");

        Debug.Log($"Angle: {angle}");
        Debug.Log($"Impulse delay time: {time_to_tracked_object + time_to_first_receiver}");
        Debug.Log($"Time to tracked object: {time_to_tracked_object}");
        Debug.Log($"Time to echo to reach first receiver: {time_to_first_receiver}");
        Debug.Log($"Time to echo to reach second receiver: {time_to_second_receiver}");
        Debug.Log($"Real Obj Pos: {hit_collider.transform.position}");
        Debug.Log($"Found Obj Pos: {obj_pos}");
    }
    public void StartScuttering()
    {
        if (antennas.Count < 2)
            return;

        scutter_ray_mesh_builder.BuildMesh(Vector3.Distance(antennas[0].transform.position, antennas[antennas.Count - 1].transform.position), beamwidth_angle, max_distance);
        scutter_collider.RegisterCallback(OnScutterColliderHit);
        current_angle = 60;
        radar_image_plane_material.SetFloat("scutter_ray_broadness_angle", beamwidth_angle);
        
        max_time = Time.timeAsDouble + max_distance / LightSpeed;

        StartCoroutine(TimeCounter());
    }
    public void StopScuttering()
    {
        StopCoroutine(TimeCounter());
    }


    IEnumerator TimeCounter()
    {
        NextStep();
        //Debug.Log(Time.timeAsDouble);
        yield return new WaitUntil(() => Time.timeAsDouble > max_time);

        StartCoroutine(TimeCounter());
    }


    public void NextStep()
    {
        ScutterToAngle(current_angle);
        current_angle -= beamwidth_angle / 3;
        

        if (current_angle < -60)
            current_angle = 60;
    }

    double time;
    double max_time;
    void ScutterToAngle(float angle)
    {
        radar_image_plane_material.SetFloat("scutter_ray_angle", current_angle + 90);
        scutter_collider.transform.rotation = Quaternion.Euler(0, current_angle, 0);
    }
    public void ClearLog()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
