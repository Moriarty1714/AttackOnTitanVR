using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //transform.Rotate(new Vector3(0, 1, 0), Random.Range(0, 360));
        float tmpScaleXZ = Random.Range(8.0f, 15.0f);
        float tmpScaleY = Random.Range(8.0f, 30.0f);
        //transform.localScale = new Vector3(tmpScaleXZ, tmpScaleY, tmpScaleXZ);
    }
}
