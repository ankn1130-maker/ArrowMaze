using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer line;

    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;

        line.SetPosition(0, new Vector3(0, 0, 0));
        line.SetPosition(1, new Vector3(0, 1, 0));
    }
}
