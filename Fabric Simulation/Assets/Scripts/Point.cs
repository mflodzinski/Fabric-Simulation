using UnityEngine;

public class Point
{
    public Vector2 pos, oldPos;
    public bool locked;
    float offset = 0.1f;
    public Point(Vector2 pos)
    {
        this.pos = pos;
        float randomOffsetX = Random.Range(-offset, offset);
        float randomOffsetY = Random.Range(-offset, offset);
        oldPos = new Vector2(pos.x + randomOffsetX, pos.y + randomOffsetY);
    }
}
