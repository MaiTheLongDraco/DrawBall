using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectBall : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private bool isDrawing = false;
    private bool isEnterBall=false;
    private BallController _ballControl;
    private int holder;
    private GameObject startPoint;
    private GameObject endPoint;
   
    private void Awake()
    {
        _ballControl=GetComponent<BallController>();
        startPoint = this.gameObject;

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&& isEnterBall)
        {
            StartDrawing();
        }
        else if (Input.GetMouseButtonDown(0)&& !isEnterBall || Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }

        if (isDrawing)
        {
            UpdateLine();
        }
        CheckMouseOnBall();
    }

    private void StartDrawing()
    {
        isDrawing = true;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, GetMouseWorldPosition());
        startPoint = this.gameObject;
        //endPoint = null;
    }

    private void StopDrawing()
    {
        isDrawing = false;
    }

    private void UpdateLine()
    {
        int positionCount = lineRenderer.positionCount;
        lineRenderer.positionCount = positionCount + 1;
        lineRenderer.SetPosition(positionCount, GetMouseWorldPosition());
        holder = lineRenderer.positionCount+1;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; 
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
    private void CheckMouseOnBall()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                var dot = hit.collider.gameObject.GetComponent<BallController>();
                endPoint = hit.collider.gameObject;
           if(dot.type== _ballControl.type&&startPoint!=endPoint)
                {
                    Debug.Log($"startPoint {startPoint.name} endPoint {endPoint.name}");
                    Debug.Log("same type");
                   // StopDrawing();
                }
             
            }
        }

    }
    private void OnMouseEnter()
    {
        isEnterBall = true;

    }
    private void OnMouseExit()
    {
        isEnterBall = false;    
    }
    
}
