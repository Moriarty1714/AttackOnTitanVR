using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class Proba : MonoBehaviour
{
    [SerializeField] GameObject hitGameObject;
    [SerializeField] Rigidbody rb;
    [SerializeField] bool leftHand;
    LineRenderer myLine;

    Vector3 hitPoint;
    bool grabbed = false;

    public InputActionReference click = null;

    // Start is called before the first frame update
    void Start()
    {
        myLine = GetComponent<LineRenderer>();
        myLine.startWidth = 0.02f;
        myLine.endWidth = 0.02f;
        myLine.positionCount = 2;

        click.action.started += Shoot;
        click.action.canceled += Shoot3;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 100f, Color.cyan);
        myLine.SetPosition(0, transform.position);
        if (!grabbed)
            myLine.SetPosition(1, transform.position + transform.forward * 200f);
        else
            myLine.SetPosition(1, hitPoint);


        //print(Input.GetAxis("LeftGrab"));
        //print(Input.GetAxis("RightGrab"));
        //print(Input.GetKey(KeyCode.JoystickButton14));
        //print(Input.GetKey(KeyCode.JoystickButton15));

        //if (leftHand)
        //{
        //    if (Input.GetKeyDown(KeyCode.JoystickButton15))
        //        Shoot();
        //    else if (Input.GetKey(KeyCode.JoystickButton15))
        //        Shoot2();
        //    else if (Input.GetKeyUp(KeyCode.JoystickButton15))
        //        Shoot3();
        //}
        //else
        //{
        //    if (Input.GetKeyDown(KeyCode.JoystickButton14))
        //        Shoot();
        //    else if (Input.GetKey(KeyCode.JoystickButton14))
        //        Shoot2();
        //    else if (Input.GetKeyUp(KeyCode.JoystickButton14))
        //        Shoot3();
        //}

    }

    private void FixedUpdate()
    {
        if (grabbed)
            rb.AddForce((hitPoint - transform.position).normalized * 100f, ForceMode.Force);
        if (rb.velocity.magnitude >= 50)
            rb.velocity *= 0.95f;
    }

    public void Shoot(InputAction.CallbackContext obj)
    {
        print("Shoot");
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 200f))
        {
            if (hit.transform.tag == "Surface")
            {
                Debug.Log("Estoy hitting");
                hitPoint = hit.point;
                grabbed = true;
            }
        }
    }

    public void Shoot3(InputAction.CallbackContext obj)
    {
        grabbed = false;
    }
}
