using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, 1, 0), Random.Range(0, 360));
        transform.Rotate(new Vector3(1,0,0), -90);
        float tmpScale = Random.Range(8.0f, 12.0f);
        transform.localScale = new Vector3(tmpScale, tmpScale, tmpScale);
    }
}
