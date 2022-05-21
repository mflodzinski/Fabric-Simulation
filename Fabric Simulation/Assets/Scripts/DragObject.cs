using UnityEngine;

public class DragObject : MonoBehaviour
{
    Vector3 offset;

    private void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 offset = transform.position - mousePos;
    }

    private void OnMouseDrag()
    {
        if (FindObjectOfType<Fabric>().simulate)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetComponent<Ball>().point.pos = mousePos + offset;
        }
    }
}
