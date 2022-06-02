using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class LanzaRelampago : MonoBehaviour
{
    const float SHOOT_FORCE_MULTIPLIER = 4000f;
    InputActionReference shootTrigger = null;
    [SerializeField] private InputActionReference shootClick;
    private bool isHoldingDown = false;
    private bool isClicking = false;
    Rigidbody rb;
    [SerializeField]
    ParticleSystem explosionParticles;
    private ActionBasedController xrL;
    private ActionBasedController xrR;
    [SerializeField] private AudioSource fireSource;
    [SerializeField] private AudioSource explSource;
    [SerializeField] private AudioSource fireRightSource;
    [SerializeField] private AudioClip fireAudioClip;
    [SerializeField] private AudioClip explAudioClip;
    private LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        xrL = GameObject.Find("LeftBaseController").GetComponent<ActionBasedController>();
        xrR = GameObject.Find("RightBaseController").GetComponent<ActionBasedController>();
        line = this.GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (shootTrigger != null)
		{
            shootTrigger.action.performed += CheckShootProjectile;
            shootClick.action.started += Aim;
            shootClick.action.canceled += Shoot;
		}

        if (isHoldingDown && isClicking)
        {
            ShootFlagActive();
        }
        else
        {
            line.startColor = new Color(line.startColor.r, line.startColor.g, line.startColor.b, 0f);
        }
    }

    public void CheckShootProjectile(InputAction.CallbackContext action)
    {
        Vector2 aux = action.ReadValue<Vector2>();
        if (aux.y < -0.5/* && (
        Vector3.Distance(this.transform.position, xrL.transform.position) < 0.5f ||  
        Vector3.Distance(this.transform.position, xrR.transform.position) < 0.5f)*/)
		{
			/*if ( Vector3.Distance(this.transform.position, xrL.transform.position) < 0.37f){
				fireSource.PlayOneShot(fireAudioClip);
				explSource.PlayOneShot(explAudioClip);
				
			}
			else{
				fireRightSource.PlayOneShot(fireAudioClip);
				explSource.PlayOneShot(explAudioClip);
			}*/
            isHoldingDown = true;
        }
        else
        {
            isHoldingDown = false;
        }
    }

    public void GetDevice(InputActionReference _shootTrigger)
    {
        if(Vector3.Distance(this.transform.position, xrL.transform.position) < 0.4f ||
        Vector3.Distance(this.transform.position, xrR.transform.position) < 0.4f)
            shootTrigger = _shootTrigger;
    }

    public void DeselectDevice()
	{
        shootTrigger = null;
	}
    
    public void Shoot(InputAction.CallbackContext action)
	{
		if (isHoldingDown)
		{
            Destroy(this.GetComponent<XRGrabInteractable>());
            rb.AddRelativeForce(Vector3.forward * SHOOT_FORCE_MULTIPLIER);
            rb.useGravity = false;
            DeselectDevice();
            line.SetPosition(1, this.transform.position);
        }

        isClicking = false;
    }

    public void Aim(InputAction.CallbackContext action)
    {
        isClicking = true;
    }

    private void OnCollisionEnter(Collision collision)
	{
        if(collision.gameObject.tag == "Sliceable" || collision.gameObject.tag == "Surface")
		    if (isHoldingDown)
		    {
                Instantiate(explosionParticles).transform.position = this.transform.position;
                if(collision.gameObject.TryGetComponent<Diana>(out var diana))
			    {
                    Destroy(diana.transform.parent.gameObject);
			    }
                Destroy(this.gameObject);
		    }
	}

    void ShootFlagActive()
	{
        line.startColor = new Color(line.startColor.r, line.startColor.g, line.startColor.b, 1f);
        line.SetPosition(0, this.transform.position);
        line.SetPosition(1, this.transform.position + this.transform.forward * 200);
    }
}
