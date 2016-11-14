using UnityEngine;
using System.Collections;

public class Node
{
    public bool Value = false;
    public Vector2 Material = new Vector2(0.25f,0.25f);

    public Node(bool val, int matx, int maty)
    {
        Value = val;
        Material = new Vector2(0.25f+matx*0.5f,0.25f+maty*0.5f);
    }

    public Node()
    {
        
    }                
}                              
