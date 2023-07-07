using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private List<Connect> _connects;
    // Start is called before the first frame update
    #region Close foor modification
    private BallController _startBall;
    private BallController _secondBall;
    private CheckLineIntersection _checkLineIntersection;
    private List<BallController> _balls;
    private List<BallController> _connectedBall;
    [SerializeField] private float _distance;
    private bool isDrawing;
    private bool _connected;
    private bool isEnterBall = false;
    private int _currentLineIndex = 0;
    private LineType _lineType;
    private Segment _currentSegment = new Segment(Vector2.one * -1f, Vector2.one * -1f);
    [SerializeField] private int _numberOfDraw;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private List<LineRenderer> _lines;
    [SerializeField] private List<LineRenderer> _usedLine;
    // [SerializeField]private Dictionary<int,LineRenderer> dict_lines;
    [SerializeField] private int _resetDrawNumber;
    //[SerializeField] private float _timePerDraw;
    //[SerializeField] private float reset_timePerDraw;
    [SerializeField] private float _threshHold;
    private bool _canUpdateLine;
    private LimitDrawRange limitDrawRange;

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
    public List<LineRenderer> UsedLine { get => _usedLine; set => _usedLine = value; }
    public List<Connect> Connects
    {
        get
        {
            if (_connects == null) _connects = new List<Connect>(); return _connects;
        }
        set => _connects = value;
    }


    //public Dictionary<int, LineRenderer> Dict_lines { get => dict_lines; set => dict_lines = value; }
    #endregion

    void Start()
    {
        _line =FindObjectOfType<LineRenderer>();
        _lines = FindObjectsOfType<LineRenderer>().ToList();
    }
    private void Awake()
    {
        _balls = new List<BallController>();
        _connectedBall = new List<BallController>();
        _checkLineIntersection= new CheckLineIntersection(this);
        limitDrawRange=GetComponent<LimitDrawRange>();  
    }
    void Update()
    {
        Draw();
    }
    private LineRenderer GetCurrentLine()
    {
        // just edit here  
        Debug.Log(Lines.IndexOf(Line) + " CurrentLineIndex");
        return Lines[Lines.IndexOf(Line)];
    }
    // get existing line on the scene except current Line
    public List<LineRenderer> GetExistingLines()
    {
        var result = new List<LineRenderer>();
        result.AddRange(Lines);
        result.Remove(GetCurrentLine());
        return result;
    }
   
    private void UpdateLineIndex()
    {
        _line = _lines[_currentLineIndex];
        _lineType = _line.GetComponent<LineType>();
    }
    private void Draw()
    {
        var check = CheckIsDrawing();
        if (!isEnterBall) return;
        if (check)
        {
            IsHitBall();
            HandleCanUpdateLine();
            UpdateLineIndex();
             HandleIfIntersecting();
        }
    }
    private void HandleIfIntersecting()
    {
        var isIntersect = _checkLineIntersection.IsIntersectOtherLine(_currentSegment);
        if (isIntersect)
        {
            // CanUpdateLine = false;
            //Line.positionCount = 0;
        }
    }
   
    // =================================this segment handle draw line==================================
    private void HandleCanUpdateLine()
    {
        if (!CanUpdateLine)
            return;
        UpdateLine();
    }

    private bool CanUpdateDraw()
    {
        if (_currentSegment.GetLeght() < _threshHold) return false;
        return true;
    }

    private void UpdateLine()
    {
      // if (!CanUpdateDraw()) return;
        var hasStop = limitDrawRange.HasStopped;
        Debug.Log($"hasStop {hasStop}");
        if(hasStop)
        {
            var position = _line.positionCount;
            _line.positionCount = position ;
            _line.SetPosition(position, limitDrawRange.StopPosition);
        }
        else
        {
            var position = _line.positionCount;
            _line.positionCount = position + 1;
            _line.SetPosition(position, GetNewPoint());
            DrawDebugTest(_currentSegment);
        }
    }

    private void StopDraw()
    {
        isDrawing = false;
        var hasStop = limitDrawRange.HasStopped;
        if (!hasStop)
        {
            limitDrawRange.StopPosition = GetNewPoint();
            limitDrawRange.HasStopped = true;
        }

    }
    private void ResetLine()
    {
        _line.positionCount = 0;
    }

    private void StartDraw()
    {
        isDrawing = true;
        _line.positionCount = 1;
        _line.SetPosition(0, GetNewPoint());
        if (!limitDrawRange.HasStopped)
        {
            limitDrawRange.StopPosition = GetNewPoint();
            limitDrawRange.HasStopped = true;
        }
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

    [SerializeField]
    List<Vector3> _lastSegment;
    List<GameObject> _lastSegmentGO;

    [SerializeField]
    private GameObject _debugPoint;
    
    public void DrawDebugTest(Segment segment)
    {
        Debug.Log($"IS _lastSegmentGO != null {_lastSegmentGO != null}");
        if (_lastSegmentGO != null)
            foreach (var go in _lastSegmentGO)
                Destroy(go);
        _lastSegment = new List<Vector3> { segment.startPoint, segment.endPoint };
        _lastSegmentGO = new List<GameObject>();
        StartCoroutine(CreateDebugLine(segment.startPoint, segment.endPoint));
        Debug.Log($"call: {segment.startPoint} - {segment.endPoint}");
        Debug.DrawLine(segment.startPoint, segment.endPoint);
    }
    private IEnumerator CreateDebugLine(Vector3 segmentStart, Vector3 segmentEnd)
    {
        if (_lastSegmentGO != null)
            foreach (var go in _lastSegmentGO)
                Destroy(go);
        yield return new WaitForSeconds(0.005f);
        var Test1 = Instantiate(_debugPoint, segmentStart, Quaternion.identity);
        Test1.transform.position = segmentStart;
        yield return new WaitForSeconds(0.005f);
        var Test2 = Instantiate(_debugPoint, segmentEnd, Quaternion.identity);
        Test2.transform.position = segmentEnd;
        _lastSegmentGO.Add(Test1);
        _lastSegmentGO.Add(Test2);
    }


    public void ShowDebugPoint(Vector2 point, Color color)
    {
        var debugPoint = Instantiate(_debugPoint, point, Quaternion.identity);
        debugPoint.GetComponent<DebugPoint>().SetColor(color);
        debugPoint.name = "crossed";
        Debug.LogError($"crossed: {point}");
        debugPoint.transform.position = point;
    }
    public Vector3 GetNewPoint()
    {
        var pos = Input.mousePosition;
        pos.z = 10f;
        var point = Camera.main.ScreenToWorldPoint(pos);
        _currentSegment = new Segment(_currentSegment.endPoint, point);
        return point;
    }

    private void IsHitBall()
    {
        Vector2 mousePos = GetNewPoint();
        RaycastHit2D hit;
        hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            _secondBall = hit.collider.gameObject.GetComponent<BallController>();
            if (_secondBall.transform == _startBall.transform) return;
            HandleListConnectedBall(_secondBall);
            HandleIfSameType();
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
        
        Vector2 mousePos = GetNewPoint();
        if (_startBall.type == _secondBall.type && _line.positionCount > 0)
        {
            if (Vector2.Distance(_line.GetPosition(_line.positionCount - 1), _secondBall.transform.position) <= _distance)
            {
                _line.positionCount++;
                _line.SetPosition(_line.positionCount - 1, _secondBall.transform.position);
                _connected = true;
                HandleUsedLine();
                StopDraw();
                ResetDrawNumber();
            }
        } else if (_startBall.type != _secondBall.type) {
            StopDraw();
            ResetLine();
        }
        else
        {
            _connected = false;
            Debug.Log("dont Have the same type");
        }
    }

    private void AddConnectedBall()
    {
        Connect connect = new Connect();
        var isAdded = connect.balls.Contains(_startBall.transform) || connect.balls.Contains(_secondBall.transform);
        if (isAdded)
            return;
            connect.balls.Add(_startBall.transform);
            connect.balls.Add(_secondBall.transform);
        connect.line=_line;
        AddNewConnect(connect);
    }

    private void HandleUsedLine()
    {
        if (_connected && !_usedLine.Contains(_line))
        {
            _usedLine.Add(_line);
            AddConnectedBall();
        }
    }

    private void HandleIfOverLineNeed()
    {
        var isOverLine = IsOverNeededLine();
        if (isOverLine )
        {
            //_lines.First().enabled = false;
            return;
        }
        CreateNewLine();
    }
    
    public void GetNewLine()
    {
        HandleIfOverLineNeed();
    }
    private void ResetDrawNumber()
    {
        _numberOfDraw = _resetDrawNumber;
    }
    private bool IsOverNeededLine()
    {
        // Just change the condition of check overLine from _lines.Count > _balls.Count to _lines.Count >= _balls.Count
        if (_lines.Count >= _balls.Count)
        {
            return true;
        }
        else return false;
    }
    private void CreateNewLine()
    {
        Debug.Log(_numberOfDraw + " numberofdraw");
        if (_numberOfDraw < 0) return;
        var line = Instantiate(_linePrefab, GetNewPoint(), Quaternion.identity);
        line.name = "New Line " ;
        _line = FindObjectOfType<LineRenderer>();
        _lines = FindObjectsOfType<LineRenderer>().ToList();
    }
 
    public void ReturnResetLine()
    {
        ResetLine();
    }

    public void AddNewConnect(Connect connect)
    {
        Connects.Add(connect);
    }
}

