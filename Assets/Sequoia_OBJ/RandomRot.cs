using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, 1, 0), Random.Range(0, 360));
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }
}
