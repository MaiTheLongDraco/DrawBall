using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BallController;

public class LineSwich : MonoBehaviour
{
    [SerializeField] private GameController _gameControl;
    private Color _thisColor;


    // Start is called before the first frame update
    void Start()
    {
        _gameControl = FindObjectOfType<GameController>();
        _thisColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        HandleAddConBallPermission();
        SetLineColor();
    }
    private void IndentifyLineType()
    {
        var lineType = _gameControl.Line.GetComponent<LineType>().lineType;
        var _ballType=this.GetComponent<BallController>();
        switch (_ballType.type)
        {
            case BallType.BLUE_BALL: break;
            case BallType.GREEN_BALL:
                {
                    lineType = LineType._lineType.GREEN_LINE;
                    _gameControl.Line.GetComponent<LineType>().lineType = lineType;
                }
                break;
            case BallType.YELLOW_BALL: break;
            case BallType.RED_BALL:
                {
                    lineType = LineType._lineType.RED_LINE;
                    _gameControl.Line.GetComponent<LineType>().lineType = lineType;
                }
                break;
        }
    }
   
    private void SetLineColor()
    {
        _gameControl.Line.SetColors(_thisColor, _thisColor);
    }
    private void HandleAddConBallPermission()
    {
        foreach(var b in _gameControl.Connects)
        {
            var isContain = b.balls.Contains(this.transform);
            if(isContain)
            {
                _gameControl.CurrentLineIndex = _gameControl.Lines.IndexOf(b.line);
                _gameControl.Line = b.line;
                _gameControl.Line.positionCount = 0;
                IndentifyLineType();
                RemoveConnectedBall(b.balls);
                _gameControl.CanUpdateLine = true;
            }
        }
    }
    private void RemoveConnectedBall(List<Transform> balls)
    {
        foreach(var b in balls)
        {
            var ballType=b.GetComponent<BallController>().type;
           _gameControl.ConnectedBall.RemoveAll(ball=>ball.type==ballType);
        }
    }    
}

