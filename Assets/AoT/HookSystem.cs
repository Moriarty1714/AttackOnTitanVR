using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class HookSystem : MonoBehaviour
{
    enum State { NONE, SHOOT, PULL }

    [Header("References")]
    public GameObject target;
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

    [Header("Variables")]
    public bool leftGrabbed = false;
    public bool rightGrabbed = false;

    public float force = 70f;
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

        // Actions
        clickLeft.action.started += ClickLeft;
        clickLeft.action.canceled += ReleaseLeft;
        clickRight.action.started += ClickRight;
        clickRight.action.canceled += ReleaseRight;

        clickLeft2.action.started += CutLeft;
        clickRight2.action.started += CutRight;
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


        // Left controller
        if (leftState != State.NONE)
        {
            leftHookLine.SetPosition(1, leftHitPoint);
            
        }
        else
        {
            leftHookLine.SetPosition(1, leftHook.transform.position);
            
        }

        // Right controller
        if (rightState != State.NONE)
        {
            rightHookLine.SetPosition(1, rightHitPoint);
            
        }
        else
        {
            rightHookLine.SetPosition(1, rightHook.transform.position);
            
        }


        if (Physics.Raycast(waist.transform.position, rb.velocity, out RaycastHit hit, 10f))
        {
            rb.velocity *= 0.99f;
        }
    }

    private void FixedUpdate()
    {
        if (leftState == State.PULL)
            rb.AddForce((leftHitPoint - leftController.transform.position).normalized * force, ForceMode.Force);
        if (rightState == State.PULL)
            rb.AddForce((rightHitPoint - rightController.transform.position).normalized * force, ForceMode.Force);

        if (rb.velocity.magnitude >= maxSpeed)
            rb.velocity *= 0.95f;
    }

    public void ClickLeft(InputAction.CallbackContext obj)
    {
        print("Shoot");
        if (leftState != State.NONE)
        {
            leftState = State.NONE;
            return;
        }
        if (Physics.Raycast(leftController.transform.position, leftController.transform.forward, out RaycastHit hit, 200f))
        {
            if (hit.transform.tag == "Surface")
            {
                Debug.Log("Estoy hitting");
                leftHitPoint = hit.point;
                leftState = State.SHOOT;

                StartCoroutine(LaunchHook());
            }
        }
        
    }
    public void ClickRight(InputAction.CallbackContext obj)
    {
        if (rightState != State.NONE)
        {
            rightState = State.NONE;
            return;
        }
        print("Shoot");
        if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out RaycastHit hit, 200f))
        {
            if (hit.transform.tag == "Surface")
            {
                Debug.Log("Estoy hitting");
                rightHitPoint = hit.point;
                rightState = State.SHOOT;

                StartCoroutine(LaunchHook());
            }
        }
    }

    public void ReleaseLeft(InputAction.CallbackContext obj)
    {
        leftState = State.PULL;
    }
    public void ReleaseRight(InputAction.CallbackContext obj)
    {
        rightState = State.PULL;
    }

    public void CutLeft(InputAction.CallbackContext obj)
    {
        leftState = State.NONE;
    }
    public void CutRight(InputAction.CallbackContext obj)
    {
        rightState = State.NONE;
    }

    private IEnumerator LaunchHook()
    {
        leftHookLine.SetPosition(1, leftHitPoint);
        yield break;
    }
}
