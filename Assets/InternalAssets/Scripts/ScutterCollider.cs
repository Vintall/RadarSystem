using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScutterCollider : MonoBehaviour
{
    Action<Collider> callback;

    public void RegisterCallback(Action<Collider> callback) => this.callback = callback;
    private void OnTriggerEnter(Collider other) => this.callback?.Invoke(other);
}
