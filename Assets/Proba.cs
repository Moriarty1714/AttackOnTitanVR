using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class Proba : MonoBehaviour
{
    [SerializeField] GameObject hitGameObject;
    LineRenderer myLine;
    // Start is called before the first frame update
    void Start()
    {
        myLine = GetComponent<LineRenderer>();
        myLine.startWidth = 0.02f;
        myLine.endWidth = 0.02f;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 100f, Color.cyan);
        myLine.SetPosition(0, transform.position);
        myLine.SetPosition(1, transform.position + transform.forward * 100f);

        
    }

    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            Debug.Log("Estoy dentro");
            hitGameObject.transform.position = hit.point;
        }
    }
}
