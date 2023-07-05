using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour
{
    [SerializeField] private GameController _gameControl;
    [SerializeField] private CheckLineIntersection _lineIntersect;
    public enum BallType
    {
        RED_BALL,
        BLUE_BALL,
        GREEN_BALL,
        YELLOW_BALL
    }
    public BallType type;
    [SerializeField] private Color _colorToParse;
    [SerializeField] private string _hexaColor;
    void Start()
    {
        _gameControl = FindObjectOfType<GameController>();
        _lineIntersect = new CheckLineIntersection(_gameControl);
    }
    private void OnMouseDown()
    {
        _gameControl.StartPoint = this;
        ConenectBallHandle();
        _gameControl.IsEnterBall = true;
        _gameControl.NumberOfDraw--;
        GrantAddBallOrNot();
    }
    private void OnMouseExit()
    {
    }
    private void ConenectBallHandle()
    {
        var condition = _gameControl.ConnectedBall.Contains(this);
        if (condition)
        {
           // _gameControl.ConnectedBall.RemoveAll(b=>b.type==this.type);
            _gameControl.CanUpdateLine = false;
            return;
        }
        else
        {
            _gameControl.CanUpdateLine = true;
            _gameControl.ConnectedBall.Add(this);
        }
    }
    private void GrantAddBallOrNot()
    {
        var _isSameType = _gameControl.Balls.Any(b => b.type == this.type);
        foreach (var b in _gameControl.Balls)
        {
            Debug.Log(b.type.ToString() + " Ball Type");
        }
        if (_gameControl.Balls.Contains(this) || _isSameType)
        {

            return;
        }
        _gameControl.Balls.Add(this);
    }
    private void OnMouseUp()
    {
        _gameControl.IsEnterBall = false;
    }
}
