using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPoint : MonoBehaviour
{
    [SerializeField]
    private float _radius = 0.5f;
    [SerializeField]
    private Color _color = Color.blue;
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
