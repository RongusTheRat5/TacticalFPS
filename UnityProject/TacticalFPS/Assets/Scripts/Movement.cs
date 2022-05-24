using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private Rigidbody rb;

    public Transform groundCheck;

    public float jumpHeight;
    public float gravity;
    public float xAcc;
    public float xDeAcc;
    public float xMax;
    public float zAcc;
    public float zDeAcc;
    public float zMax;

    public float crouchAcc;
    public float crouchMaxSpeed;

    private bool spaceLastFrame = false;
    private bool cLastFrame = false;
    private bool ctrlLastFrame = false;
    private bool crouching = false;
    private bool prone = false;

    public float sprintAcc;
    public float sprintMaxSpeed;
    public float aimAcc;
    public float aimMaxSpeed;

    private float yVel;
    private float xVel;
    private float zVel;

    private bool[] currentInputs = new bool[9];
    private Vector3 targetPos;

    public Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime * 6f);

        //Debug, inputs will be received from each player, calculated, and then position will be sent back to all other players.
        currentInputs[0] = Input.GetKey(KeyCode.W);
        currentInputs[1] = Input.GetKey(KeyCode.A);
        currentInputs[2] = Input.GetKey(KeyCode.S);
        currentInputs[3] = Input.GetKey(KeyCode.D);
        currentInputs[4] = Input.GetKey(KeyCode.Space);
        currentInputs[5] = Input.GetKey(KeyCode.LeftShift);
        currentInputs[6] = Input.GetMouseButton(1);
        currentInputs[7] = Input.GetKey(KeyCode.C);
        currentInputs[8] = Input.GetKey(KeyCode.LeftControl);
    }

    private void FixedUpdate()
    {
        bool[] inputs = currentInputs;
        currentInputs = new bool[9];

        Vector2 moveDirection = new Vector2();
        moveDirection.y += inputs[0] ? 1 : 0;
        moveDirection.y -= inputs[2] ? 1 : 0;
        moveDirection.x += inputs[3] ? 1 : 0;
        moveDirection.x -= inputs[1] ? 1 : 0;

        MoveState currentState = MoveState.Idle;

        bool grounded = Physics.Raycast(controller.transform.position - (Vector3.up * 0.95f), Vector3.down, out RaycastHit _hit, 0.1f);
        bool aimed = grounded && inputs[6];
        bool changecrouched = !cLastFrame && grounded && inputs[7];
        bool changeprone = !ctrlLastFrame && grounded && inputs[8];
        bool sprint = inputs[5] && moveDirection.y > 0 && !aimed && !crouching && !prone;


        if (changeprone && !prone && crouching) crouching = !crouching;
        if (changecrouched && !crouching && prone) prone = !prone;

        if (changecrouched) crouching = !crouching;
        if (changeprone) prone = !prone;

        if (moveDirection.magnitude == 0 && !crouching) currentState = MoveState.Idle;
        if (moveDirection.magnitude != 0 && !crouching && !sprint) currentState = MoveState.Walking;
        if (sprint) currentState = MoveState.Running;
        if (prone) currentState = MoveState.Prone;
        if (crouching) currentState = MoveState.Crouched;

        switch (currentState)
        {
            case MoveState.Walking:
                targetPos = new Vector3(0f, 0.73f, 0f);
                xVel += aimed ? aimAcc * moveDirection.y : xAcc * moveDirection.y;
                xVel = aimed ? Mathf.Clamp(xVel, -aimMaxSpeed, aimMaxSpeed) : Mathf.Clamp(xVel, -xMax, xMax);
                zVel += aimed ? aimAcc * moveDirection.x : zAcc * moveDirection.x;
                zVel = aimed ? Mathf.Clamp(zVel, -aimMaxSpeed, aimMaxSpeed) : Mathf.Clamp(zVel, -zMax, zMax);
                break;
            case MoveState.Running:
                targetPos = new Vector3(0f, 0.73f, 0f);
                xVel += sprintAcc;
                xVel = Mathf.Clamp(xVel, 0f, sprintMaxSpeed);
                zVel += zAcc * moveDirection.x;
                zVel = Mathf.Clamp(zVel, -zMax, zMax);
                break;
            case MoveState.Crouched:
                targetPos = new Vector3(0, 0.35f, 0);
                xVel += aimed ? aimAcc * moveDirection.y : crouchAcc * moveDirection.y;
                xVel = aimed ? Mathf.Clamp(xVel, -aimMaxSpeed, aimMaxSpeed) : Mathf.Clamp(xVel, -crouchMaxSpeed, crouchMaxSpeed);
                zVel += aimed ? aimAcc * moveDirection.x : crouchAcc * moveDirection.x;
                zVel = aimed ? Mathf.Clamp(zVel, -aimMaxSpeed, aimMaxSpeed) : Mathf.Clamp(zVel, -crouchMaxSpeed, crouchMaxSpeed);
                break;
            case MoveState.Prone:
                targetPos = new Vector3(0, 0f, 0);
                xVel += aimed ? aimAcc / 2 * moveDirection.y : crouchAcc / 2 * moveDirection.y;
                xVel = aimed ? Mathf.Clamp(xVel, -aimMaxSpeed / 2, aimMaxSpeed / 2) : Mathf.Clamp(xVel, -crouchMaxSpeed / 2, crouchMaxSpeed / 2);
                zVel += aimed ? aimAcc / 2 * moveDirection.x : crouchAcc / 2 * moveDirection.x;
                zVel = aimed ? Mathf.Clamp(zVel, -aimMaxSpeed / 2, aimMaxSpeed / 2) : Mathf.Clamp(zVel, -crouchMaxSpeed / 2, crouchMaxSpeed / 2);
                break;
            case MoveState.Idle:
                targetPos = new Vector3(0f, 0.73f, 0f);
                break;
            default:
                break;
        }

        GetComponent<Sway>().AnimateBasedOnState(currentState);

        if (moveDirection.y == 0)
        {
            xVel = Mathf.MoveTowards(xVel, 0f, xDeAcc);
        }

        if (moveDirection.x == 0)
        {
            zVel = Mathf.MoveTowards(zVel, 0f, zDeAcc);
        }

        yVel -= gravity;

        if (grounded)
        {
            yVel = inputs[4] && !spaceLastFrame ? Mathf.Sqrt(gravity * 2f * jumpHeight) : -_hit.distance;
        }

        Vector2 xzvelocity = new Vector2(xVel, zVel);
        xzvelocity = Vector2.ClampMagnitude(xzvelocity, sprint ? sprintMaxSpeed : xMax);

        Vector3 movement = new Vector3();
        movement += transform.TransformDirection(xzvelocity.x * Vector3.forward);
        movement += transform.TransformDirection(xzvelocity.y * Vector3.right);
        movement += yVel * Vector3.up;

        spaceLastFrame = inputs[4];
        cLastFrame = inputs[7];
        ctrlLastFrame = inputs[8];

        controller.Move(movement);
    }


}

public enum MoveState
{
    Running,
    Walking,
    Crouched,
    Idle,
    Prone
}


//InputPayload used to store inputs on each fixed update;
[System.Serializable]
public class InputPayload
{
    public bool[] inputs;
    public Vector3 forward;

    public InputPayload()
    {
        inputs = new bool[8];
        forward = Vector3.zero;
    }

    public InputPayload(bool[] inputs, Vector3 forward)
    {
        this.inputs = inputs;
        this.forward = forward;
    }
}

//StatePayload used to store states to compare with server position;
[System.Serializable]
public class StatePayload
{
    public Vector3 position;
    public bool jumpPressed;
    public MoveState state;

    public StatePayload()
    {
        position = Vector3.zero;
        jumpPressed = false;
        state = MoveState.Idle;
    }
    
    public StatePayload(Vector3 position, bool jumpPressed, MoveState state)
    {
        this.position = position;
        this.jumpPressed = jumpPressed;
        this.state = state;
    }
}