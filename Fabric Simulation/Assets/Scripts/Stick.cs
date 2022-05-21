using UnityEngine;

public class Stick
{
    public Point pointA, pointB;
    public float lenght;
    public Stick(Point pointA, Point pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        lenght = Vector2.Distance(pointA.pos, pointB.pos);
    }
}
