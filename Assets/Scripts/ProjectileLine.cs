using System.Collections.Generic;
using UnityEngine;

public class ProjectileLine : MonoBehaviour
{
    static public ProjectileLine S;

    [Header("Set in Inspector")]
    public float minDist = 0.1f;
    public List<LineRenderer> lines;

    private int currentLine;
    private GameObject _poi;
    private List<Vector3> points;

    void Awake()
    {
        S = this;
        foreach (var item in lines)
        {
            item.enabled = false;
        }
        currentLine = 0;
        points = new List<Vector3>();
    }

    public GameObject poi
    {
        get
        {
            return _poi;
        }
        set
        {
            _poi = value;
            if (_poi != null)
            {
                currentLine++;
                if (currentLine == 3)
                    currentLine = 0;

                lines[currentLine].enabled = false;
                points = new List<Vector3>();
                AddPoint();
            }
        }
    }

    public void Clear()
    {
        _poi = null;
        foreach (var item in lines)
        {
            item.enabled = false;
        }
        points = new List<Vector3>();
    }

    public void AddPoint()
    {
        if (points.Count == 20)
            return;

        Vector3 pt = _poi.transform.position;
        if (points.Count > 0 && (pt - lastPoint).magnitude < minDist)
        {
            return;
        }
        if (points.Count == 0)
        {
            Vector3 launchPosDiff = pt - Slingshot.LAUNCH_POS;
            points.Add(pt + launchPosDiff);
            points.Add(pt);
            lines[currentLine].positionCount = 2;

            lines[currentLine].SetPosition(0, points[0]);
            lines[currentLine].SetPosition(1, points[1]);

            lines[currentLine].enabled = true;
        }
        else
        {
            points.Add(pt);
            lines[currentLine].positionCount = points.Count;
            lines[currentLine].SetPosition(points.Count - 1, lastPoint);
            lines[currentLine].enabled = true;
        }
    }

    public Vector3 lastPoint
    {
        get
        {
            if (points.Count == 0)
            {
                return (Vector3.zero);
            }
            return points[points.Count - 1];
        }
    }
    void FixedUpdate()
    {
        if (poi == null)
        {
            if (FollowCam.POI != null)
            {
                if (FollowCam.POI.tag == "Projectile")
                {
                    poi = FollowCam.POI;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        AddPoint();
        if (FollowCam.POI == null)
        {
            poi = null;
        }
    }

}
