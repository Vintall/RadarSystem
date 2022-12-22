using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadarListNode : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI node_index;
    [SerializeField] TextMeshProUGUI x_position;
    [SerializeField] TextMeshProUGUI y_position;
    [SerializeField] TextMeshProUGUI radial_speed;

    public int index;

    public void InitializeNode(int index, Vector2 pos, double radial_speed)
    {
        this.index = index;
        node_index.text = index.ToString();
        x_position.text = pos.x.ToString();
        y_position.text = pos.y.ToString();
        this.radial_speed.text = radial_speed.ToString();
    }
    public void OnMoveToDetailsButtonPressed() => UIController.Instance.OnMoveToDetailsButtonPressed(index);
}
