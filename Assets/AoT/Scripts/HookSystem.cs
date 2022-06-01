using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using DG.Tweening;
public class HookSystem : MonoBehaviour
{
    public enum State { NONE, SHOOT, PULL }

    [Header("References")]
    public Rigidbody rb;

    public GameObject leftController;
    public GameObject rightController;
    private LineRenderer leftControllerLine;
    private LineRenderer rightControllerLine;

    public GameObject waist;
    public GameObject body;
    public GameObject head;
    public GameObject leftHook;
    public GameObject rightHook;
    private LineRenderer leftHookLine;
    private LineRenderer rightHookLine;
    public GameObject leftHookEnd;
    public GameObject rightHookEnd;

    [Header("Variables")]
    public bool leftGrabbed = false;
    public bool rightGrabbed = false;
    public float leftHookDistance;
    public float rightHookDistance;

    public float force = 25f;
    public float maxSpeed = 20f;

    private Vector3 leftHitPoint, rightHitPoint;
    public State leftState;
    public State rightState;
    private float timerLeftHook = 0f;
    private float timerRightHook = 0f;

    private Vector3 previousPos;

    public InputActionReference clickLeft = null;
    public InputActionReference clickRight = null;
    public InputActionReference clickLeft2 = null;
    public InputActionReference clickRight2 = null;

    [SerializeField] private ActionBasedController xrL;
    [SerializeField] private ActionBasedController xrR;

    [SerializeField] private AudioSource hookLeftSource;
    [SerializeField] private AudioSource hookRightSource;
    [SerializeField] public AudioClip hookClip;
    
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
        clickLeft2.action.canceled += PullEndLeft;
        clickRight2.action.performed += PullRight;
        clickRight2.action.canceled += PullEndRight;

        //Haptics
        //InputDevices.GetDevices(inputDevices);

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

        Debug.DrawLine(waist.transform.position, waist.transform.position+ rb.velocity.normalized * 10f,Color.black);
        if (Physics.Raycast(waist.transform.position, rb.velocity.normalized, out RaycastHit hit, 5f))
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
        {
            rb.AddForce((leftHitPoint - leftController.transform.position).normalized * force, ForceMode.Acceleration);
            leftHookDistance = Vector3.Distance(leftHitPoint, body.transform.position);
            xrL.SendHapticImpulse(0.2f, 0.2f);
            hookLeftSource.Play();
        }
        if (rightState == State.PULL)
        {
            rb.AddForce((rightHitPoint - rightController.transform.position).normalized * force, ForceMode.Acceleration);
            rightHookDistance = Vector3.Distance(rightHitPoint, body.transform.position);
            xrR.SendHapticImpulse(0.2f, 0.2f);
            hookRightSource.Play();
        }

        if (leftState == State.SHOOT)
        {
            hookLeftSource.Pause();
            // For�a cap al ganxo
            rb.AddForce((leftHitPoint - leftController.transform.position).normalized*rb.velocity.magnitude, ForceMode.Acceleration);

            // Re-calibra la posici� per a que el ganxo no s'allargui ni s'acurti
            if (Vector3.Distance(leftHitPoint, body.transform.position) > leftHookDistance)
            {
                Vector3 newPos = leftHitPoint + (body.transform.position - leftHitPoint).normalized * leftHookDistance;
                Vector3 newVelocity = (newPos - previousPos).normalized * rb.velocity.magnitude;

                /*if (rb.velocity.magnitude > 0.1f)
                {
                    float angle = Mathf.Acos(Vector3.Dot(newVelocity.normalized, rb.velocity.normalized));
                    head.GetComponentInParent<XROrigin>().RotateAroundCameraUsingOriginUp(angle * Mathf.Rad2Deg);
                    print(angle * Mathf.Rad2Deg);
                }*/

                body.transform.position = newPos;
                rb.velocity = newVelocity;
            }

        }
        if (rightState == State.SHOOT)
        {
            hookRightSource.Pause();
            // For�a cap al ganxo
            rb.AddForce((rightHitPoint - rightController.transform.position).normalized * rb.velocity.magnitude, ForceMode.Acceleration);

            // Re-calibra la posici� per a que el ganxo no s'allargui ni s'acurti
            if (Vector3.Distance(rightHitPoint, body.transform.position) > rightHookDistance)
            {
                Vector3 newPos = rightHitPoint + (body.transform.position - rightHitPoint).normalized * rightHookDistance;
                Vector3 newVelocity = (newPos - previousPos).normalized * rb.velocity.magnitude;

                /*if (rb.velocity.magnitude > 0.1f)
                {
                    float angle = Mathf.Acos(Vector3.Dot(newVelocity.normalized, rb.velocity.normalized));
                    head.GetComponentInParent<XROrigin>().RotateAroundCameraUsingOriginUp(angle * Mathf.Rad2Deg);
                    print(angle * Mathf.Rad2Deg);
                }*/

                body.transform.position = newPos;
                rb.velocity = newVelocity;
            }
            //print(rightHookDistance);
        }

        if (rb.velocity.magnitude >= maxSpeed && (leftState != State.NONE || rightState != State.NONE))
            rb.velocity *= 0.95f;

        previousPos = body.transform.position;
    }

    private void ShootLeft(InputAction.CallbackContext obj)
    {
        Shoot(ref leftState, ref leftController, ref leftHitPoint, ref leftHookLine, ref leftHookEnd, ref leftHookDistance, ref xrL);
    }
    private void ShootRight(InputAction.CallbackContext obj)
    {
        Shoot(ref rightState, ref rightController, ref rightHitPoint, ref rightHookLine, ref rightHookEnd, ref rightHookDistance, ref xrR);
    }
    private void Shoot(ref State state, ref GameObject controller, ref Vector3 hitPoint, ref LineRenderer hookLine, ref GameObject hookEnd, ref float hookDistance, ref ActionBasedController vibController)
    {
        if (Physics.Raycast(controller.transform.position, controller.transform.forward, out RaycastHit hit, 400f))
        {
            if (hit.transform.tag == "Surface")
            {
                hitPoint = hit.point;
                state = State.SHOOT;
                hookDistance = Vector3.Distance(hitPoint, body.transform.position);

                hookEnd.transform.DOMove(hitPoint, 0.3f);

                rb.mass = 0.25f;

                vibController.SendHapticImpulse(1.0f, 0.1f);
            }
        }
    }

    private void PullLeft(InputAction.CallbackContext obj)
    {
        if (leftState == State.SHOOT)
        {
            leftState = State.PULL;
            timerLeftHook = Time.time;
        }
    }
    private void PullEndLeft(InputAction.CallbackContext obj)
    {
        if (leftState == State.PULL)
        {
            leftState = State.SHOOT;
        }
    }
    private void PullRight(InputAction.CallbackContext obj)
    {
        if (rightState == State.SHOOT)
        {
            rightState = State.PULL;
            timerRightHook = Time.time;
        }
    }
    private void PullEndRight(InputAction.CallbackContext obj)
    {
        if (rightState == State.PULL)
        {
            rightState = State.SHOOT;
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
                //rb.AddForce(vel * force / 6f, ForceMode.VelocityChange);
            }
        }
    }

    private IEnumerator SetMassInAir()
    {
        yield return new WaitForSeconds(2f);
        rb.mass = 10f;
    }

    //private void pulse() {
    //    foreach (var device in inputDevices)
    //    {
    //        HapticCapabilities capabilities;
    //        if (device.TryGetHapticCapabilities(out capabilities))
    //        {
    //            if (capabilities.supportsImpulse)
    //            {
    //                uint channel = 0;
    //                float amplitude = 0.5f;
    //                float duration = 1.0f;
    //                device.SendHapticImpulse(channel, amplitude, duration);
    //            }
    //        }
    //    }
    //    xr.SendHapticImpulse(0.7f, 2f);
    //}
}
