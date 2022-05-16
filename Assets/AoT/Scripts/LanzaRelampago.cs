using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class LanzaRelampago : MonoBehaviour
{
    const float SHOOT_FORCE_MULTIPLIER = 200f;
    InputActionReference shootTrigger;
    private bool isShooted = false;
    Rigidbody rb;
    ParticleSystem explosionParticles;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (shootTrigger != null)
		{
            shootTrigger.action.started += CheckShootProjectile;
		}
    }

    public void CheckShootProjectile(InputAction.CallbackContext action)
    {
        if(action.ReadValue<Vector2>() == Vector2.down)
		{
            Shoot();
        }
    }

    public void GetDevice(InputActionReference _shootTrigger)
    {
        shootTrigger = _shootTrigger;
        DeselectDevice();
    }

    public void DeselectDevice()
	{
        shootTrigger = null;
	}
    
    public void Shoot()
	{
        isShooted = true;
        rb.AddRelativeForce(Vector3.forward * SHOOT_FORCE_MULTIPLIER);
        rb.useGravity = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
        explosionParticles.Play();
        Destroy(this.gameObject);
	}
}
