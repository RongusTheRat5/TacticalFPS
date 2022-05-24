using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float clampAngle = 85f;

    [HideInInspector] public float verticalRotation;
    [HideInInspector] public float horizontalRotation;

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = transform.root.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        float mouseVertical = -Input.GetAxis("Mouse Y");
        float mouseHorizontal = Input.GetAxis("Mouse X");

        horizontalRotation += mouseHorizontal * sensitivity * Time.deltaTime;

        transform.root.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        verticalRotation += mouseVertical * sensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);



    }
}
