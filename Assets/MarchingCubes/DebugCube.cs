using UnityEngine;
using System.Collections;

public class DebugCube : MonoBehaviour {

    public int edgeCase;
	public Vector3[] p;
    public bool[] val;
    
    

    void Awake()
    {
        p = new Vector3[8];
        val = new bool[8];
    }

}
