using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    // Used for vertical lines to prevent division by 0 errors
    const float verticalLineGradient = 1e5f;

    float gradient;
    float y_intercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    float gradientPerpendicular;

    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if(dx == 0) // Prevent division by 0 errors
        {
            gradientPerpendicular = verticalLineGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;
        }

        if(gradientPerpendicular == 0) // Prevent division by 0 errors
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }

        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    bool GetSide(Vector2 p)
    {
        return ((p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y)) >
            ((p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x));
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    /*
     * Returns the closest distance between a point and this line
     */
    public float DistanceFromPoint(Vector2 p)
    {
        float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
        float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
        float intersectY = gradient * intersectX + y_intercept;

        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 lineDirection = new Vector3(1f, 0f, gradient).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, 0f, pointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine(lineCenter - lineDirection * length / 2.0f, 
            lineCenter + lineDirection * length / 2.0f);
    }
}
