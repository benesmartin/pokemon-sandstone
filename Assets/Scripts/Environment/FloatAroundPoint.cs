using UnityEngine;

public class FloatAroundPoint : MonoBehaviour
{
    public Vector3 centerPoint;
    public float radius = 2.0f;
    public float speed = 1.0f;

    private float angle;

    void Update()
    {
        angle += speed * Time.deltaTime;
        var offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;
        transform.position = centerPoint + offset;
    }
}

