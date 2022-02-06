
using UnityEngine;
using Objects;

public class OrbitCamara : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 5f;
    [SerializeField]
    float smoothFactor = 5f;

    private Vector3 lookAt = new Vector3();
    Vector3 offsetCam;
    private void Start()
    {
        offsetCam = transform.position - Vector3.zero;
    }

    void LateUpdate()
    {
        
        
            Point barry = CloudPointsManager.Instance.GetBarrycenter();
            if (barry.Position != lookAt)
            {
                lookAt = barry.Position;
                offsetCam = transform.position - barry.Position;
                transform.LookAt(lookAt);
            }
        
        if (lookAt != Vector3.zero && Input.GetMouseButton(1))
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
            offsetCam = camTurnAngle * offsetCam;

            Vector3 newPos = barry.Position + offsetCam;
            
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
            transform.LookAt(lookAt);
        }
    }
}