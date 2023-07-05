using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public struct Segment
{
    public Vector2 startPoint;
    public Vector2 endPoint;

    public Segment(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    public float GetLeght() => Vector2.Distance(startPoint, endPoint);
}

public class CheckLineIntersection 
{
    private GameController gameController;

    public CheckLineIntersection(GameController gameController)
    {
        this.gameController = gameController;
    }

    private void print(string message)
    {
        Debug.Log(message);
    }

    public bool IsIntersectOtherLine(Segment segment)
    {
        var otherLines = GetOtherLines();
        // iterate throungh other line and check whether current line segment have intersect with other Line
        foreach(var line in otherLines)
        {
            if( IsCrossLine(segment, line))
            {
                print($"invalid {line.GetComponent<LineType>().lineType} + {gameController.Lines.IndexOf(line)}");
                return true;
            }
        }
        print("valid");
        return false;
    }

    private List<LineRenderer> GetOtherLines()
    {
        return gameController.GetExistingLines();
    }
    public bool IsCrossLine(Segment segment, LineRenderer line)
    {
        int segments = line.positionCount - 1;

        // Iterate through each segment of lineRenderer2
        for (int j = 0; j < segments; j++)
        {
            Vector3 lineRenderer2Start = line.GetPosition(j);
            Vector3 lineRenderer2End = line.GetPosition(j + 1);

            // Check for intersection between the two line segments
            if (AreSegmentsIntersecting(segment.startPoint, segment.endPoint, lineRenderer2Start, lineRenderer2End))
            {
                return true; // Lines cross over
            }
        }

        return false; // Lines do not cross over
    }

    private bool AreSegmentsIntersecting(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
    {
        float den = ((end2.y - start2.y) * (end1.x - start1.x)) - ((end2.x - start2.x) * (end1.y - start1.y));
        float num1 = ((end2.x - start2.x) * (start1.y - start2.y)) - ((end2.y - start2.y) * (start1.x - start2.x));
        float num2 = ((end1.x - start1.x) * (start1.y - start2.y)) - ((end1.y - start1.y) * (start1.x - start2.x));

        // Check if the segments are parallel or coincident
        if (Mathf.Approximately(den, 0f))
        {
            // Segments are parallel or coincident, so check if they overlap
            if (Mathf.Approximately(num1, 0f) && Mathf.Approximately(num2, 0f))
            {
                // Segments are coincident (they lie on the same line)
                return Overlaps(start1, end1, start2, end2);
            }

            // Segments are parallel and do not overlap
            return false;
        }

        // Segments are not parallel, so check for intersection
        float u = num1 / den;
        float v = num2 / den;

        // Check if the intersection point is within both segments
        return (u >= 0f && u <= 1f) && (v >= 0f && v <= 1f);
    }

    private bool Overlaps(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
    {
        // Check if the segments overlap based on their projections onto the x-z plane
        float min1 = Mathf.Min(start1.x, end1.x);
        float max1 = Mathf.Max(start1.x, end1.x);
        float min2 = Mathf.Min(start2.x, end2.x);
        float max2 = Mathf.Max(start2.x, end2.x);

        return max1 >= min2 && max2 >= min1;
    }

}
