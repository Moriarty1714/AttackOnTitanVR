using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrees : MonoBehaviour
{
    public Transform start;
    public Transform end;

    public int treeNum;

    public GameObject tree;
    void Start()
    {
        for (int i = 0; i < treeNum; i++)
        {
            GameObject tmp = Instantiate(tree, new Vector3(Random.Range(start.position.x,end.position.x), 0, Random.Range(start.position.z, end.position.z)), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
