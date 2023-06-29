using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour
{
    [SerializeField] private GameController _gameControl;
    public enum BallType
    {
        RED_BALL,
        BLUE_BALL,
        GREEN_BALL,
        YELLOW_BALL
    }
    public BallType type;
    private Color _thisColor;
    
    // Start is called before the first frame update
    void Start()
    {
        _gameControl=FindObjectOfType<GameController>();
        _thisColor=GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void LateUpdate()
    {
   
    }

    private void OnMouseDown()
    {
        IndentifyLineType();
        SetLineColor();
        _gameControl.StartPoint = this;
        ConenectBallHandle();
        _gameControl.IsEnterBall = true;
        _gameControl.NumberOfDraw--;
        GrantAddBallOrNot();
       
    }
    private void ConenectBallHandle()
    {
        var condition=_gameControl.ConnectedBall.Contains(this)/*&&_gameControl.ConnectedBall.Count>0*/;
        if (condition)
        {
            _gameControl.CanUpdateLine = false;
            return;
        }
        else
        {
            _gameControl.CanUpdateLine = true;
            _gameControl.ConnectedBall.Add(this);
        }

    }
    private void SetLineColor()
    {
        _gameControl.Line.SetColors(_thisColor, _thisColor);
    }

    private void GrantAddBallOrNot()
    {
        var _isSameType = _gameControl.Balls.Any(b => b.type == this.type);
        foreach(var b in _gameControl.Balls)
        {
            Debug.Log(b.type.ToString() + " Ball Type");
        }
        if (_gameControl.Balls.Contains(this) || _isSameType)
        {
         
            return;
        }
        _gameControl.Balls.Add(this);
    }
    private void Test()
    {
       var condition=_gameControl.ConnectedBall.Any(b=>b.type == this.type);
        if (!condition&&_gameControl.ConnectedBall.Count>0)
        {
            return;
        }
        else
        {
            Debug.Log(_gameControl.CurrentLineIndex + "_gameControl.CurrentLineIndex");
            if (_gameControl.CurrentLineIndex > _gameControl.Balls.Count - 1) return;
            Debug.Log(_gameControl.CurrentLineIndex + "_gameControl.CurrentLineIndex1");
            _gameControl.ReturnResetLine();
        _gameControl.ConnectedBall.RemoveAll(b=>b.type==this.type);
        }
    }
    private void OnMouseUp()
    {
        _gameControl.IsEnterBall = false;
    }
    private void IndentifyLineType()
    {
        var lineType = _gameControl.Line.GetComponent<LineType>().lineType;
        switch (this.type)
        {
            case BallType.BLUE_BALL: break;
            case BallType.GREEN_BALL:
                {
                    lineType= LineType._lineType.GREEN_LINE;
                    _gameControl.Line.GetComponent<LineType>().lineType = lineType;
                    SwitchIsUsingLine(lineType);
                } break;
            case BallType.YELLOW_BALL: break;
            case BallType.RED_BALL:
                {
                    lineType = LineType._lineType.RED_LINE;
                    _gameControl.Line.GetComponent<LineType>().lineType = lineType;
                    SwitchIsUsingLine(lineType);
                }
                break;
        }
    }   
    private void SwitchIsUsingLine(LineType._lineType _LineType)
    {
        var sameTypeLine = _gameControl.Lines.Find(l => l.GetComponent<LineType>().lineType == _LineType);
        var indexOfLine = _gameControl.Lines.IndexOf(sameTypeLine);
        _gameControl.CurrentLineIndex= indexOfLine;
        var type=sameTypeLine.GetComponent<LineType>().lineType;
        Debug.LogWarning(type.ToString()+ "  type.ToString()");
        _gameControl.Line=sameTypeLine;
        _gameControl.Line.positionCount = 0;
    }
   
}
