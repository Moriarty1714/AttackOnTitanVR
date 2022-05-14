using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using DG.Tweening;
public class HookSystem : MonoBehaviour
{
    enum State { NONE, SHOOT, PULL }

    [Header("References")]
    public Rigidbody rb;

    public GameObject leftController;
    public GameObject rightController;
    private LineRenderer leftControllerLine;
    private LineRenderer rightControllerLine;

    public GameObject waist;
    public GameObject leftHook;
    public GameObject rightHook;
    private LineRenderer leftHookLine;
    private LineRenderer rightHookLine;
    public GameObject leftHookEnd;
    public GameObject rightHookEnd;

    [Header("Variables")]
    public bool leftGrabbed = false;
    public bool rightGrabbed = false;

    public float force = 25f;
    public float maxSpeed = 20f;

    private Vector3 leftHitPoint, rightHitPoint;
    private State leftState;
    private State rightState;

    public InputActionReference clickLeft = null;
    public InputActionReference clickRight = null;
    public InputActionReference clickLeft2 = null;
    public InputActionReference clickRight2 = null;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize controller lines
        leftControllerLine = leftController.GetComponent<LineRenderer>();
        rightControllerLine = rightController.GetComponent<LineRenderer>();
        leftControllerLine.startWidth = rightControllerLine.startWidth = 0.01f;
        leftControllerLine.endWidth = rightControllerLine.endWidth = 0.01f;
        leftControllerLine.positionCount = rightControllerLine.positionCount = 2;

        // Initialize hook lines
        leftHookLine = leftHook.GetComponent<LineRenderer>();
        rightHookLine = rightHook.GetComponent<LineRenderer>();
        leftHookLine.startWidth = rightHookLine.startWidth = 0.03f;
        leftHookLine.endWidth = rightHookLine.endWidth = 0.03f;
        leftHookLine.positionCount = rightHookLine.positionCount = 2;

        leftHookEnd.transform.position = leftHook.transform.position;
        rightHookEnd.transform.position = rightHook.transform.position;

        // Actions
        clickLeft.action.started += ShootLeft;
        clickLeft.action.canceled += CutLeft;
        clickRight.action.started += ShootRight;
        clickRight.action.canceled += CutRight;

        clickLeft2.action.started += PullLeft;
        clickRight2.action.started += PullRight;
    }

    // Update is called once per frame
    void Update()
    {
        leftControllerLine.SetPosition(0, leftController.transform.position);
        rightControllerLine.SetPosition(0, rightController.transform.position);
        leftControllerLine.SetPosition(1, leftController.transform.position + leftController.transform.forward * 200f);
        rightControllerLine.SetPosition(1, rightController.transform.position + rightController.transform.forward * 200f);

        leftHookLine.SetPosition(0, leftHook.transform.position);
        rightHookLine.SetPosition(0, rightHook.transform.position);
        leftHookLine.SetPosition(1, leftHookEnd.transform.position);
        rightHookLine.SetPosition(1, rightHookEnd.transform.position);

        // Left controller
        if (leftState == State.NONE)
        {
            leftHookEnd.transform.position = leftHook.transform.position;
        }

        // Right controller
        if (rightState == State.NONE)
        {
            rightHookEnd.transform.position = rightHook.transform.position;
        }


        if (Physics.Raycast(waist.transform.position, rb.velocity, out RaycastHit hit, 10f))
        {
            rb.velocity *= 0.99f;
        }
    }

    private void FixedUpdate()
    {
        if (leftState == State.PULL)
            rb.AddForce((leftHitPoint - leftController.transform.position).normalized * force, ForceMode.Acceleration);
        if (rightState == State.PULL)
            rb.AddForce((rightHitPoint - rightController.transform.position).normalized * force, ForceMode.Acceleration);

        if (rb.velocity.magnitude >= maxSpeed)
            rb.velocity *= 0.95f;
    }

    private void ShootLeft(InputAction.CallbackContext obj)
    {
        Shoot(ref leftState, ref leftController, ref leftHitPoint, ref leftHookLine, ref leftHookEnd);
    }
    private void ShootRight(InputAction.CallbackContext obj)
    {
        Shoot(ref rightState, ref rightController, ref rightHitPoint, ref rightHookLine, ref rightHookEnd);
    }
    private void Shoot(ref State state, ref GameObject controller, ref Vector3 hitPoint, ref LineRenderer hookLine, ref GameObject hookEnd)
    {
        if (Physics.Raycast(controller.transform.position, controller.transform.forward, out RaycastHit hit, 400f))
        {
            if (hit.transform.tag == "Surface")
            {
                hitPoint = hit.point;
                state = State.SHOOT;

                hookEnd.transform.DOMove(hitPoint, 0.3f);
            }
        }
    }

    private void PullLeft(InputAction.CallbackContext obj)
    {
        if (leftState == State.SHOOT)
            leftState = State.PULL;
    }
    private void PullRight(InputAction.CallbackContext obj)
    {
        if (rightState == State.SHOOT)
            rightState = State.PULL;
    }

    private void CutLeft(InputAction.CallbackContext obj)
    {
        Cut(leftHook, leftHookEnd, ref leftState);
    }
    private void CutRight(InputAction.CallbackContext obj)
    {
        Cut(rightHook, rightHookEnd, ref rightState);
    }
    private void Cut(GameObject hook, GameObject hookEnd, ref State state)
    {
        state = State.NONE;
        hookEnd.transform.DOMove(hook.transform.position, 0.3f);

        //rb.AddForce((rb.velocity).normalized * force, ForceMode.Impulse);
    }
}
