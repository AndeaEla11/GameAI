using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public Transform playerCharacter;
    Vector3 cameraOffset;


    void Start()
    {
        if (playerCharacter != null)
            cameraOffset = transform.position - playerCharacter.position;
    }


    public void SetPlayer(Transform player)
    {
        playerCharacter = player;
        cameraOffset = transform.position - playerCharacter.position;
    }

    void LateUpdate()
    {
        if (playerCharacter == null) return;

        transform.position = playerCharacter.position + cameraOffset;
    }
}
