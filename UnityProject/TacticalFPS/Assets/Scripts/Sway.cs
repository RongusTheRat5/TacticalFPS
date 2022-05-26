using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{

    public Weapon currentWeapon;

    public float swayIntensity;
    public float swaySmoothness;

    public float idleBreathCoefficient;
    public float idleBreathSpeedCoefficient;
    public float walkBreathCoefficient;
    public float walkBreathSpeedCoefficient;
    public float runBreathCoefficient;
    public float runBreathSpeedCoefficient;

    public Transform cameraTransform;
    public Camera weaponCamera;
    public Transform weaponSwayParent;
    public Transform weaponParent;
    private Quaternion weapon_origin_rotation;
    private Vector3 weapon_origin_position;
    private Vector3 camera_origin_position;

    private float breathStrength;
    private float breathSpeed;

    [HideInInspector] public bool aim = false;

    private void Start()
    {
        weapon_origin_rotation = weaponSwayParent.localRotation;
        weapon_origin_position = weaponSwayParent.localPosition;
        camera_origin_position = cameraTransform.localPosition;



        breathStrength = idleBreathCoefficient;
        breathSpeed = idleBreathSpeedCoefficient;
    }

    private void Update()
    {

        UpdateSway();
        UpdateBreath();
    }



    private void UpdateSway()
    {
        float t_x_mouse = Input.GetAxisRaw("Mouse X");
        float t_y_mouse = Input.GetAxisRaw("Mouse Y");

        if (Input.GetMouseButton(1)) t_x_mouse /= 5;
        if (Input.GetMouseButton(1)) t_y_mouse /= 5;

        Quaternion t_x_adj = Quaternion.AngleAxis(swayIntensity * t_x_mouse, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(-swayIntensity * t_y_mouse, Vector3.right);
        Quaternion t_z_adj = Quaternion.AngleAxis(-2 * swayIntensity * t_x_mouse, Vector3.forward);
        Quaternion target_rotation = weapon_origin_rotation * t_x_adj * t_y_adj * t_z_adj;


        weaponSwayParent.localRotation = Quaternion.Slerp(weaponSwayParent.localRotation, target_rotation, Time.deltaTime * swaySmoothness);
    }

    private void UpdateBreath()
    {
        if (aim)
        {
            breathSpeed /= 5;
            breathStrength /= 15;
        }

        float yOffset = Mathf.Sin(Time.time * breathSpeed);
        float xOffset = Mathf.Cos(Time.time / 2 * breathSpeed);
        float zOffset = Mathf.Cos(Time.time / 4 * breathSpeed);
        Vector3 offSet = new Vector3(xOffset * breathStrength, yOffset * breathStrength, zOffset * breathStrength);

        weaponSwayParent.localPosition = Vector3.Lerp(weaponSwayParent.localPosition, weapon_origin_position + offSet, (aim) ? Time.deltaTime * 8f : Time.deltaTime * breathSpeed);
        //cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, camera_origin_position - (offSet), Time.deltaTime * breathSpeed);
    }

    public void AnimateBasedOnState(MoveState state)
    {
        switch (state)
        {
            case MoveState.Idle:
                SetBreathStrength(idleBreathCoefficient, idleBreathSpeedCoefficient);
                return;
            case MoveState.Walking:
                SetBreathStrength(walkBreathCoefficient, walkBreathSpeedCoefficient);
                return;
            case MoveState.Running:
                SetBreathStrength(runBreathCoefficient, runBreathSpeedCoefficient);
                return;
            case MoveState.Crouched:
                SetBreathStrength(idleBreathCoefficient, idleBreathSpeedCoefficient);
                return;
            default:
                return;
        }
    }

    private void SetBreathStrength(float strength, float speed)
    {
        breathStrength = strength / 100;
        breathSpeed = speed;
    }
}
