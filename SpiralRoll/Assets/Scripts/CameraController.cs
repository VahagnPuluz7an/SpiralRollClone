using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smootTime;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smootTime);
    }
}
