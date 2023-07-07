using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitDrawRange : MonoBehaviour
{
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    private GameController _gamControl;
    private Vector3 stopPosition;
    private bool hasStopped = false;

    public Vector3 StopPosition { get => stopPosition; set => stopPosition = value; }
    public bool HasStopped { get => hasStopped; set => hasStopped = value; }

    // Start is called before the first frame update
    void Start()
    {
        minX = -4.9f;
        maxX = 4.9f;
        minY = -4.02f;
        maxY = 5.717f;
    }
    private void Awake()
    {
        _gamControl = GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        MakeLimit();
    }
    private void MakeLimit()
    {
        var mousePos = _gamControl.GetNewPoint();
        var hzCondition = mousePos.x >= minX && mousePos.x <= maxX;
        var vtCondition = mousePos.y >= minY && mousePos.y <= maxY;
        if (hzCondition && vtCondition)
        {
            HasStopped = false;
            Debug.Log("Player draw in range ");
        }
        else
        {
            Debug.Log("Player  not draw in range ");
            HasStopped = true;
            HandleLineAtThreshHold(mousePos);
        }
    }
    private void HandleLineAtThreshHold(Vector3 mousePos1)
    {
        if (_gamControl.Line == null) return;
        var index = _gamControl.Line.positionCount - 1;
        var mousePos = _gamControl.GetNewPoint();

    }
}
  
