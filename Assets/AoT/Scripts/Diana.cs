using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diana : MonoBehaviour
{
    public bool isMetal;
    [SerializeField]
    Material metalMat;
    [SerializeField]
    Material woodMat;
    // Start is called before the first frame update
    void Start()
    {
		if (isMetal)
		{
            this.GetComponent<MeshRenderer>().material = metalMat;
            this.tag = "Surface";
            Destroy(this.GetComponent<Sliceable>());
		}
		else
		{
            this.GetComponent<MeshRenderer>().material = woodMat;
        }
    }


}
