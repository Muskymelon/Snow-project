using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.LookAt(target.transform.position);
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}
