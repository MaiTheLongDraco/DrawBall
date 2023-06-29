using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    #region Close foor modification
    private BallController _startBall;
    private BallController _secondBall;
    private List<BallController> _balls;
    private List<BallController> _connectedBall;
    [SerializeField] private float _distance;
    private bool isDrawing;
    private bool _connected;
    private bool isEnterBall = false;
    private int _currentLineIndex = 0;
    private LineType _lineType;

    [SerializeField] private int _numberOfDraw;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private List<LineRenderer> _lines;
    [SerializeField] private List<LineRenderer> _usedLine;
    // [SerializeField]private Dictionary<int,LineRenderer> dict_lines;
    [SerializeField] private int _resetDrawNumber;
    private bool _canUpdateLine;

    #endregion
    #region Public for extension
    public BallController StartPoint { get { return _startBall; } set { _startBall = value; } }
    public BallController EndPoint { get { return _secondBall; } set { _secondBall = value; } }

    public bool IsEnterBall { get => isEnterBall; set => isEnterBall = value; }
    public int NumberOfDraw { get => _numberOfDraw; set => _numberOfDraw = value; }
    public int CurrentLineIndex { get => _currentLineIndex; set => _currentLineIndex = value; }
    public List<BallController> Balls { get => _balls; set => _balls = value; }
    public LineRenderer Line { get => _line; set => _line = value; }
    public bool Connected { get => _connected; set => _connected = value; }
    public List<BallController> ConnectedBall { get => _connectedBall; set => _connectedBall = value; }
    public LineType LineType { get => _lineType; set => _lineType = value; }
    public List<LineRenderer> Lines { get => _lines; set => _lines = value; }
    public bool CanUpdateLine { get => _canUpdateLine; set => _canUpdateLine = value; }

    //public Dictionary<int, LineRenderer> Dict_lines { get => dict_lines; set => dict_lines = value; }
    #endregion

    void Start()
    {
        CreateNewLine();
        //_line =FindObjectOfType<LineRenderer>();
        UpdateLineIndex();
        _lines = FindObjectsOfType<LineRenderer>().ToList();
    }
    private void Awake()
    {
        _balls = new List<BallController>();
        _connectedBall = new List<BallController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLineIndex();
        Draw();
    }
    private void UpdateLineIndex()
    {
        //_line = dict_lines[_currentLineIndex];
        _line = _lines[_currentLineIndex];
        _lineType = _line.GetComponent<LineType>();
    }
    private void Draw()
    {
        var check = CheckIsDrawing();
        if (!isEnterBall) return;
        if (check)
        {
            // CreateNewLine();
            IsHitBall();
            HandleCanUpdateLine();
        }
        Debug.Log(isEnterBall + " isEnterBall");

    }
    private void HandleCanUpdateLine()
    {
        if (!_canUpdateLine)
            return;
            UpdateLine();
        
    }

    private void UpdateLine()
    {

        var position = _line.positionCount;
        _line.positionCount = position + 1;
        _line.SetPosition(position, GetMousePos());
    }


    private void StopDraw()
    {
        isDrawing = false;
    }
    private void ResetLine()
    {
        _line.positionCount = 0;
    }

    private void StartDraw()
    {
        isDrawing = true;
        _line.positionCount = 1;
        _line.SetPosition(0, GetMousePos());
    }
    private bool CheckIsDrawing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDraw();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopDraw();
            if (!_connected)
            {
                ResetLine();
                Debug.Log(isEnterBall + " isEnterBall");
            }
        }
        return isDrawing;
    }

    private Vector3 GetMousePos()
    {
        var pos = Input.mousePosition;
        pos.z = 10f;
        return Camera.main.ScreenToWorldPoint(pos);
    }
    private void IsHitBall()
    {
        Vector2 mousePos = GetMousePos();
        RaycastHit2D hit;
        hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            // _numberOfDraw--;
            _secondBall = hit.collider.gameObject.GetComponent<BallController>();
            HandleListConnectedBall(_secondBall);
            Debug.Log(_connectedBall.Count + " _connectedBall.Count");
            HandleIfSameType();
            Debug.Log("startPoint " + _startBall.name);
            Debug.Log("secondPoint " + _secondBall.name);
        }
        else
        {
            _connected = false;
        }
    }
    private void HandleListConnectedBall(BallController _secondBall)
    {
        var condition = _connectedBall.Contains(_secondBall);
        if (condition) return;
        _connectedBall.Add(_secondBall);
    }
    private void HandleIfSameType()
    {
        Vector2 mousePos = GetMousePos();
        if (_startBall.type == _secondBall.type)
        {
            Debug.Log("Have the same type");
            //if(Vector2.Distance(mousePos,_secondBall.transform.position)<= _distance)
            if (Vector2.Distance(_line.GetPosition(_line.positionCount - 1), _secondBall.transform.position) <= _distance)
            {
                Debug.DrawLine(mousePos, _secondBall.transform.position, Color.red);
                _line.positionCount++;
                _line.SetPosition(_line.positionCount - 1, _secondBall.transform.position);
                _connected = true;
                HandleUsedLine();
                StopDraw();
                ResetDrawNumber();
                HandleIfOverLineNeed();
                GrabNextLine();
            }

        }
        else
        {
            _connected = false;
            Debug.Log("dont Have the same type");
            //ResetLine();
            ReturnResetLine();
        }
    }
    private void HandleUsedLine()
    {
        if(_connected)
        {
            _usedLine.Add(_line);
        }
    }

    private void HandleIfOverLineNeed()
    {
        var isOverLine = IsOverNeededLine();
        if (isOverLine)
        {
            _lines.First().enabled= false;
            return;
        }
        CreateNewLine();
    }
    private void ResetDrawNumber()
    {
        _numberOfDraw = _resetDrawNumber;
    }
    private void GrabNextLine()
    {
        //  var isLineIndexOver = _currentLineIndex > _balls.Count - 1;
        var isLineIndexOver = _currentLineIndex < 0;
        if (!_connected || isLineIndexOver) return;
        //_currentLineIndex--;
        // _currentLineIndex= _balls.Count - 1;
        _currentLineIndex = _lines.Count - 2;
        //_currentLineIndex = dict_lines.Count - 2;
    }
    private bool IsOverNeededLine()
    {
        // if (_balls.Count < _lines.Count-1)
        if (_lines.Count > _balls.Count)
        //if (_balls.Count < dict_lines.Count)
        {
            return true;
        }


        else return false;
    }
    private void CreateNewLine()
    {
        Debug.Log(_numberOfDraw + " numberofdraw");
        if (_numberOfDraw <= 0) return;
        var line = Instantiate(_linePrefab, GetMousePos(), Quaternion.identity);
        line.name = "New Line";
        _lines = FindObjectsOfType<LineRenderer>().ToList();


    }
    private void CheckLineIntersect()
    {
        for (int i = 0; i < _lines[_currentLineIndex + 1].positionCount; i++)
        {
            for (int j = 0; j < _lines[_currentLineIndex].positionCount; j++)
            {
                var l1 = _lines[_currentLineIndex + 1].GetPosition(i);
                var l2 = _lines[_currentLineIndex].GetPosition(j);

            }
        }
    }
    public bool CheckVector3ListsOverlap(List<Vector3> list1, List<Vector3> list2)
    {
        bool overlap = list1.Any(v1 => list2.Any(v2 => AreVectorsEqual(v1, v2)));
        return overlap;
    }

    public bool AreVectorsEqual(Vector3 v1, Vector3 v2)
    {
        float epsilon = 0.0001f;
        return Mathf.Abs(v1.x - v2.x) < epsilon
            && Mathf.Abs(v1.y - v2.y) < epsilon
            && Mathf.Abs(v1.z - v2.z) < epsilon;
    }
    public void ReturnResetLine()
    {
        ResetLine();
    }

    //private void HandleLine()
    //{
    //    if(_connected)
    //    {
    //        _lines.Remove(_line);
    //    }
    //}
}
