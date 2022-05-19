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
    private float timerLeftHook = 0f;
    private float timerRightHook = 0f;

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

        clickLeft2.action.performed += PullLeft;
        clickRight2.action.performed += PullRight;
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

        Debug.DrawLine(waist.transform.position, waist.transform.position+ rb.velocity.normalized * 5f,Color.black);
        if (Physics.Raycast(waist.transform.position, rb.velocity.normalized, out RaycastHit hit, 15f))
        {
            if (hit.transform.tag == "Surface")
                rb.velocity *= 0.965f;
        }

        if (leftState != State.NONE && rightState != State.NONE) 
        {
            rb.drag = 1f;
        }
        else 
        {
            rb.drag = 0.1f;
        }
    }

    private void FixedUpdate()
    {
        if (leftState == State.PULL)
            rb.AddForce((leftHitPoint - leftController.transform.position).normalized * force, ForceMode.Acceleration);
        if (rightState == State.PULL)
            rb.AddForce((rightHitPoint - rightController.transform.position).normalized * force, ForceMode.Acceleration);

        if (rb.velocity.magnitude >= maxSpeed && (leftState != State.NONE || rightState != State.NONE))
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

                rb.mass = 0.25f;
            }
        }
    }

    private void PullLeft(InputAction.CallbackContext obj)
    {
        if(leftState != State.NONE && obj.ReadValue<float>() <= 0.1f)
        {
            leftState = State.SHOOT;
        }
        else
        {
            if (leftState == State.SHOOT)
            {
                leftState = State.PULL;
                timerLeftHook = Time.time;
            }
        }
    }
    private void PullRight(InputAction.CallbackContext obj)
    {
        if (rightState != State.NONE && obj.ReadValue<float>() <= 0.1f)
        {
            rightState = State.SHOOT;
        }
        else
        {
            if (rightState == State.SHOOT)
            {
                rightState = State.PULL;
                timerRightHook = Time.time;
            }
        }
    }

    private void CutLeft(InputAction.CallbackContext obj)
    {
        Cut(leftHook, leftHookEnd, ref leftState, timerLeftHook);
    }
    private void CutRight(InputAction.CallbackContext obj)
    {
        Cut(rightHook, rightHookEnd, ref rightState, timerRightHook);
    }
    private void Cut(GameObject hook, GameObject hookEnd, ref State state, float timerHook)
    {
        if (state == State.NONE) return;
        State lastState = state;
        state = State.NONE;
        hookEnd.transform.DOMove(hook.transform.position, 0.3f);

        if (leftState == State.NONE && rightState == State.NONE )
        {
            if (rb.mass != 10f)
            {
                StartCoroutine(SetMassInAir());
            }
            
            if (lastState == State.PULL && (Time.time - timerHook) > 0.7f)
            {
                //Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                //vel.Normalize();
                //vel += Vector3.up*(hookEnd.transform.position - hook.transform.position).normalized.y;
                //vel.Normalize();

                Vector3 vel = hookEnd.transform.position - hook.transform.position;
                vel.Normalize();
                rb.AddForce(vel * force / 4f, ForceMode.VelocityChange);
            }
        }
    }

    private IEnumerator SetMassInAir()
    {

        yield return new WaitForSeconds(2f);
        rb.mass = 10f;
    }
}