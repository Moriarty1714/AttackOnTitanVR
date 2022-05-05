using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaistPosition : MonoBehaviour
{
    public GameObject camera;
    public float height;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camera.transform.position + Vector3.down*height;
        transform.eulerAngles = new Vector3(0f, camera.transform.eulerAngles.y, 0f);
    }
}
