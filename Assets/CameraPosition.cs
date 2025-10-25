using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    private float fixedX;
    private float fixedZ;

    void Start()
    {
        fixedX = 0;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // only follow target's Y position
        Vector3 targetPosition = new Vector3(fixedX, target.position.y, fixedZ);

        // smooth movement (optional)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
