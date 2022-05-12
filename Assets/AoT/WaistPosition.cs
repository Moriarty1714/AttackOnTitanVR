using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaistPosition : MonoBehaviour
{
    public GameObject camera;
    public float waistHeight;
    public GameObject collider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Waist position
        transform.position = camera.transform.position + Vector3.down*waistHeight;
        transform.eulerAngles = new Vector3(0f, camera.transform.eulerAngles.y, 0f);

        collider.transform.position = new Vector3(transform.position.x, collider.transform.position.y, transform.position.z);
    }
}
