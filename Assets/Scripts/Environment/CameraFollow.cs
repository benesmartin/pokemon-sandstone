using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 0.125f;
    private Vector3 offset = new Vector3(0.25f, -9.5f, -10); 

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        
        Vector3 newPosition = smoothSpeed > 0 ? Vector3.Lerp(transform.position, desiredPosition, smoothSpeed) : desiredPosition;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z); 

        
        
    }
}
