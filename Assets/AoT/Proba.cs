using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class Proba : MonoBehaviour
{
    [SerializeField] private GameObject hitGameObject;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool leftHand;
    [SerializeField] private GameObject waist;

    private LineRenderer myLine;
    private LineRenderer hook;

    Vector3 hitPoint;
    public bool grabbed = false;

    public InputActionReference click = null;

    // Start is called before the first frame update
    void Start()
    {
        myLine = GetComponent<LineRenderer>();
        myLine.startWidth = 0.02f;
        myLine.endWidth = 0.02f;
        myLine.positionCount = 2;

        hook = waist.GetComponent<LineRenderer>();
        hook.startWidth = 0.02f;
        hook.endWidth = 0.02f;
        hook.positionCount = 2;

        click.action.started += Click;
        click.action.canceled += Release;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 100f, Color.cyan);
        myLine.SetPosition(0, transform.position);
        hook.SetPosition(0, waist.transform.position);

        if (!grabbed)
        {
            myLine.SetPosition(1, transform.position + transform.forward * 200f);
            hook.SetPosition(1, waist.transform.position);
        }
        else
        {
            myLine.SetPosition(1, hitPoint);
            hook.SetPosition(1, hitPoint);
        }
    }

    private void FixedUpdate()
    {
        if (grabbed)
            rb.AddForce((hitPoint - transform.position).normalized * 70f, ForceMode.Force);
        if (rb.velocity.magnitude >= 20)
            rb.velocity *= 0.95f;
    }

    public void Click(InputAction.CallbackContext obj)
    {
        print("Shoot");
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 200f))
        {
            if (hit.transform.tag == "Surface")
            {
                Debug.Log("Estoy hitting");
                hitPoint = hit.point;
                grabbed = true;

                StartCoroutine(LaunchHook());
            }
        }
    }

    public void Release(InputAction.CallbackContext obj)
    {
        grabbed = false;
    }

    private IEnumerator LaunchHook()
    {
        hook.SetPosition(1, hitPoint);
        yield break;
    }
}
