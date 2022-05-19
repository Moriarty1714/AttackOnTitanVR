using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEspalda : MonoBehaviour
{
    [SerializeField] GameObject head;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = head.transform.position;
        this.transform.rotation = head.transform.rotation;
    }
}
