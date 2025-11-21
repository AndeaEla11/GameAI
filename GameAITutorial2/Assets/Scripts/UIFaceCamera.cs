using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{

    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }

}
