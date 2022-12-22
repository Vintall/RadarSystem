using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadarDetailNode : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI x_position;
    [SerializeField] TextMeshProUGUI y_position;

    [SerializeField] TextMeshProUGUI x_speed;
    [SerializeField] TextMeshProUGUI y_speed;

    public void InitializeNode(Vector2 pos, Vector2? speed)
    {
        x_position.text = pos.x.ToString();
        y_position.text = pos.y.ToString();

        if(speed == null)
        {
            x_speed.text = "";
            y_speed.text = "";
        }
        else
        {
            x_speed.text = speed.Value.x.ToString();
            y_speed.text = speed.Value.y.ToString();
        }

        
    }
}
