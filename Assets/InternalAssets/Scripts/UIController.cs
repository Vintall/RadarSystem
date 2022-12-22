using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    static UIController instance;
    public static UIController Instance => instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public enum CurrentWindow
    {
        List,
        Detail
    }
    public CurrentWindow current_window = CurrentWindow.List;

    [SerializeField] Transform radar_list_content;
    [SerializeField] Transform radar_details_content;

    [SerializeField] GameObject radar_list_node_prefab;
    [SerializeField] GameObject radar_detail_node_prefab;

    List<RadarListNode> radar_list_nodes = new List<RadarListNode>();
    List<RadarDetailNode> radar_detail_nodes = new List<RadarDetailNode>();


    [SerializeField] Transform full_list;
    [SerializeField] Transform details;
    
    public void AddNode(int index, Vector2 pos, double radial_speed)
    {
        RadarListNode buffer = Instantiate(radar_list_node_prefab, radar_list_content).GetComponent<RadarListNode>();
        buffer.InitializeNode(index, pos, System.Math.Round(radial_speed, 2));

        radar_list_nodes.Add(buffer);
    }
    public void RemoveNode(int index)
    {
        for (int i = 0; i < radar_list_nodes.Count; i++)
            if (radar_list_nodes[i].index == index)
            {
                Destroy(radar_list_nodes[i].gameObject);
                radar_list_nodes.RemoveAt(i);
                break;
            }
    }
    public void ChangeNode(int index, Vector2 pos, double radial_speed)
    {
        for (int i = 0; i < radar_list_nodes.Count; i++)
            if (radar_list_nodes[i].index == index)
            {
                radar_list_nodes[i].InitializeNode(index, pos, System.Math.Round(radial_speed,2));
                break;
            }
    }
    public void ReloadDetailsList(List<(Vector4, double)> pos)
    {
        for (int i = 0; i < radar_detail_nodes.Count; i++)
            Destroy(radar_detail_nodes[i].gameObject);

        radar_detail_nodes.Clear();

        for (int i = 0; i < pos.Count; i++)
        {
            RadarDetailNode node = Instantiate(radar_detail_node_prefab).GetComponent<RadarDetailNode>();
            node.transform.SetParent(radar_details_content);

            if (i == 0)
                node.InitializeNode(pos[i].Item1, null);
            else
            {
                Vector2 speed = new Vector2((float)(pos[i].Item1.x - pos[i - 1].Item1.x)/ (float)(pos[i].Item2 - pos[i - 1].Item2), (float)(pos[i].Item1.y - pos[i - 1].Item1.y)/ (float)(pos[i].Item2 - pos[i - 1].Item2));
                node.InitializeNode(pos[i].Item1, speed);
            }
            radar_detail_nodes.Add(node);
        }
    }

    void Start()
    {
        
    }
    public void OnMoveToDetailsButtonPressed(int index)
    {
        if (current_window == CurrentWindow.List)
        {
            ReloadDetailsList(RadarScutter.Instance.TrackedPoints[index].Item1);
            full_list.gameObject.SetActive(false);
            details.gameObject.SetActive(true);

            current_window = CurrentWindow.Detail;
        }
    }
    void Update()
    {
        if(current_window == CurrentWindow.Detail)
            if(Input.GetKeyDown(KeyCode.Y))
            {
                full_list.gameObject.SetActive(true);
                details.gameObject.SetActive(false);

                current_window = CurrentWindow.List;
            }
    }
}
