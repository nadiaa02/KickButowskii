using UnityEngine;

public class FollowCameraX : MonoBehaviour
{
    [SerializeField] private Transform target;   
    [SerializeField] private float smoothSpeed = 5f;

    private float startY;
    private float startZ;

    private void Start()
    {
        startY = transform.position.y;
        startZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x;
        float newX = Mathf.Lerp(transform.position.x, targetX, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, startY, startZ);
    }
}
