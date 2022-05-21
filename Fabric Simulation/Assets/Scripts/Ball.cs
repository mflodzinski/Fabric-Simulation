using UnityEngine;

public class Ball : MonoBehaviour
{
    public Point point;
    public Color initialColor;
    void Update()
    {
        transform.position = point.pos;
    }
    private void Awake()
    {
        initialColor = GetComponent<SpriteRenderer>().color;
    }
}
