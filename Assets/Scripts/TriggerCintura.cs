using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerCintura : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject head;
    public InputActionReference trigger = null;
    void Start()
    {
        trigger.action.performed += Trigger;
    }

    private void Trigger(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<float>());
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(head.transform.position.x, head.transform.position.y - 0.5f, head.transform.position.z);
        this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, head.transform.localEulerAngles.y, this.transform.rotation.z);
        

    }
}
