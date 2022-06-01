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
    const float SHOOT_FORCE_MULTIPLIER = 1000f;
    InputActionReference shootTrigger = null;
    private bool isShooted = false;
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

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        xrL = GameObject.Find("LeftBaseController").GetComponent<ActionBasedController>();
        xrR = GameObject.Find("RightBaseController").GetComponent<ActionBasedController>();
    }

    private void OnEnable()
    {
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.Space))
		{
			
            Shoot();
            
		}
        if (shootTrigger != null)
		{
            shootTrigger.action.performed += CheckShootProjectile;
		}
    }

    public void CheckShootProjectile(InputAction.CallbackContext action)
    {
        Vector2 aux = action.ReadValue<Vector2>();
        if (aux.y < -0.5 && (
        Vector3.Distance(this.transform.position, xrL.transform.position) < 0.37f ||  
        Vector3.Distance(this.transform.position, xrR.transform.position) < 0.37f))
		{
			if ( Vector3.Distance(this.transform.position, xrL.transform.position) < 0.37f){
				fireSource.PlayOneShot(fireAudioClip);
				explSource.PlayOneShot(explAudioClip);
				
			}
			else{
				fireRightSource.PlayOneShot(fireAudioClip);
				explSource.PlayOneShot(explAudioClip);
			}
            Shoot();
            DeselectDevice();
        }
    }

    public void GetDevice(InputActionReference _shootTrigger)
    {
        shootTrigger = _shootTrigger;
    }

    public void DeselectDevice()
	{
        shootTrigger = null;
	}
    
    public void Shoot()
	{
		
        Destroy(this.GetComponent<XRGrabInteractable>());
        isShooted = true;
        rb.AddRelativeForce(Vector3.forward * SHOOT_FORCE_MULTIPLIER);
        rb.useGravity = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isShooted)
		{
            Instantiate(explosionParticles).transform.position = this.transform.position;
            if(collision.gameObject.TryGetComponent<Diana>(out var diana))
			{
                Destroy(diana.transform.parent.gameObject);
			}
            Destroy(this.gameObject);
		}
	}
}
