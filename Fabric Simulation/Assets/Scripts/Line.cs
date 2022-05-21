using UnityEngine;

public class Line : MonoBehaviour
{
    public Stick stick;
    float yScale;
    float ballRadius;

    private void Start()
    {
        yScale = transform.localScale.y;
        ballRadius = FindObjectOfType<Ball>().transform.localScale.x;
    }
    void Update()
    {
        // rotation
        float angleX = stick.pointA.pos.x - stick.pointB.pos.x;
        float angleY = stick.pointA.pos.y - stick.pointB.pos.y;
        float angle = Mathf.Atan2(angleY, angleX) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        // center position
        float x = (stick.pointA.pos.x + stick.pointB.pos.x) / 2;
        float y = (stick.pointA.pos.y + stick.pointB.pos.y) / 2;
        transform.position = new Vector2(x, y);

        // length
        float distance = Mathf.Sqrt(angleX * angleX + angleY * angleY);
        transform.localScale = new Vector3(distance - ballRadius, yScale, 0);
    }
}
