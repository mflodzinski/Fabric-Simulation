using System.Collections.Generic;
using UnityEngine;

public class Fabric : MonoBehaviour
{
    public List<Point> points = new List<Point>();
    public List<Stick> sticks = new List<Stick>();

    public float width;
    public float height;
    public float gravity;
    public float bounce;
    public bool isGround;
    bool escPressed;

    [HideInInspector]
    public bool simulate;
    [HideInInspector]
    public GameObject ball;
    [HideInInspector]
    public GameObject line;

    float xOffset = 1;
    float yOffsetUp = 1;
    float yOffsetDown = 2;

    public int vertivalNum;
    public int horizontalNum;

    private void Start()
    {
        CreateWeb();
    }

    private void FixedUpdate()
    {
        if (!simulate) return;
        
        UpdatePoints();
        if (isGround)
        {
            ConstrainPoints();
        }
        UpdateSticks();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPressed = true;
        }

        else if (Input.GetKeyDown(KeyCode.C))
        {
            Clear();
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            simulate = !simulate;
        }

        else if (Input.GetMouseButtonDown(1))
        {
            LockPoint();
        }

        else if (Input.GetMouseButton(1))
        {
            CutLine();
        }

        else if (Input.GetMouseButtonDown(0) && !simulate)
        {
            if (Pointing() == null)
            {

                MakeNewPoint();
            }
            else if (Pointing().GetComponent<Ball>() != null)
            {
                JoinStickORChangePivot();
            }
        }
    }

    void LockPoint()
    {
        if (Pointing() == null) return;
        
        if (Pointing().GetComponent<Ball>() != null)
        {
            Ball b = Pointing().GetComponent<Ball>();
            Point p = b.point;
            p.locked = !p.locked;
            Color lockedPin;
            ColorUtility.TryParseHtmlString("#fd1d1d", out lockedPin);
            Pointing().GetComponent<SpriteRenderer>().color = p.locked ? lockedPin : b.initialColor;
        }
    }
    void AddAndInstantiate(Stick newStick)
    {
        sticks.Add(newStick);
        var l = Instantiate(line, Vector2.zero, Quaternion.identity);
        l.GetComponent<Line>().stick = newStick;
        l.transform.parent = transform.GetChild(0);
    }

    Collider2D Pointing()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider;
        }
        return null;
    }

    void ConstrainPoints()
    {
        foreach (Point p in points)
        {
            if (!p.locked)
            {
                var vx = p.pos.x - p.oldPos.x;
                var vy = p.pos.y - p.oldPos.y;

                if (p.pos.x > width)
                {
                    p.pos.x = width;
                    p.oldPos.x = p.pos.x + vx * bounce;
                }
                else if (p.pos.x < 0)
                {
                    p.pos.x = 0;
                    p.oldPos.x = p.pos.x + vx * bounce;
                }
                if (p.pos.y > height)
                {
                    p.pos.y = height;
                    p.oldPos.y = p.pos.y + vy * bounce;
                }
                else if (p.pos.y < 0)
                {
                    p.pos.y = 0;
                    p.oldPos.y = p.pos.y + vy * bounce;
                }
            }
        }
    }
    void UpdatePoints()
    {
        foreach (Point p in points)
        {
            if (!p.locked)
            {
                var vx = p.pos.x - p.oldPos.x;
                var vy = p.pos.y - p.oldPos.y - gravity;

                p.oldPos.x = p.pos.x;
                p.oldPos.y = p.pos.y;
                p.pos.x += vx;
                p.pos.y += vy;
            }
        }
    }

    void UpdateSticks()
    {
        foreach (Stick s in sticks)
        {

            float dx = s.pointA.pos.x - s.pointB.pos.x;
            float dy = s.pointA.pos.y - s.pointB.pos.y;
            float distance = Mathf.Sqrt(dx * dx + dy * dy);
            float diff = s.lenght - distance;
            float percent = diff / distance / 2;
            float offsetX = dx * percent;
            float offsetY = dy * percent;

            if (!s.pointA.locked)
            {
                s.pointA.pos.x += offsetX;
                s.pointA.pos.y += offsetY;
            }
            if (!s.pointB.locked)
            {
                s.pointB.pos.x -= offsetX;
                s.pointB.pos.y -= offsetY;
            }
        }
    }

    void CreateWeb()
    {
        // Create points
        float horLength = (width - 2 * xOffset) / (horizontalNum - 1);
        float verLength = (height - yOffsetUp - yOffsetDown) / (vertivalNum - 1);
        for (int i = 0; i < vertivalNum * horizontalNum; i++)
        {
            int row = (int)(i / horizontalNum);
            int col = i % horizontalNum;
            Vector2 pos = new Vector2(xOffset + horLength * col, height - yOffsetUp - verLength * row);
            Point newPoint = new Point(pos);
            points.Add(newPoint);
            var b = Instantiate(ball, newPoint.pos, Quaternion.identity);
            b.GetComponent<Ball>().point = newPoint;
            b.transform.parent = transform.GetChild(1);

            // Lock every third point
            if (row == 0 && col%3 == 0)
            {
                newPoint.locked = true;
                Color lockedPin;
                ColorUtility.TryParseHtmlString("#fd1d1d", out lockedPin);
                b.GetComponent<SpriteRenderer>().color = lockedPin;
            }
        }

        // Create sticks
        for (int i = 0; i < vertivalNum * horizontalNum - 1; i++)
        {
            if ((i + 1) % horizontalNum != 0)
            {
                Stick newStickHor = new Stick(points[i], points[i + 1]);
                AddAndInstantiate(newStickHor);
            }

            if (i < (vertivalNum - 1) * horizontalNum)
            {
                Stick newStickVer = new Stick(points[i], points[i + horizontalNum]);
                AddAndInstantiate(newStickVer);
            }
        }
    }
    void Clear()
    {
        GameObject[] lineObjects = GameObject.FindGameObjectsWithTag("Line");
        foreach (GameObject lineObj in lineObjects)
        {
            Stick stick = lineObj.GetComponent<Line>().stick;
            int index = sticks.IndexOf(stick);
            sticks.RemoveAt(index);
            Destroy(lineObj);
        }

        GameObject[] ballObjects = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ballObj in ballObjects)
        {
            Point point = ballObj.GetComponent<Ball>().point;
            int index = points.IndexOf(point);
            points.RemoveAt(index);
            Destroy(ballObj);
        }
    }

    void CutLine()
    {
        if (Pointing() != null)
        {
            if (Pointing().GetComponent<Line>() != null)
            {
                Collider2D collider = Pointing();
                Line line = collider.GetComponent<Line>();
                Stick stick = line.stick;
                int index = sticks.IndexOf(stick);
                sticks.RemoveAt(index);
                Destroy(collider.gameObject);
            }
        }
    }

    void MakeNewPoint()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Point point = new Point(cursorPos);
        points.Add(point);
        var b = Instantiate(ball, cursorPos, Quaternion.identity);
        b.GetComponent<Ball>().point = point;

        if (points.Count > 1)
        {
            Point secPoint = points[points.Count - 2];
            Stick newStick = new Stick(point, secPoint);
            AddAndInstantiate(newStick);
        }
    }

    void JoinStickORChangePivot()
    {
        Point point = Pointing().GetComponent<Ball>().point;
        // Swap points in list
        if (escPressed)
        {
            int index = points.IndexOf(point);
            Point tempPoint = points[points.Count - 1];
            points[index] = tempPoint;
            points[points.Count - 1] = point;
            escPressed = false;
        }

        // Make a new stick, add it to list and instantiate
        else
        {
            Point lastPoint = points[points.Count - 1];
            Stick newStick = new Stick(lastPoint, point);
            AddAndInstantiate(newStick);
        }
    }
}
