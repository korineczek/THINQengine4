using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

    private bool[,,] worldVals = new bool[2,2,2];
    private Chunk[,,] chunks = new Chunk[2,2,2];
    public Transform ChunkPrefab;

	// Use this for initialization
	void Start () {

	    for (int i = 0; i < chunks.GetLength(0); i++)
	    {
	        for (int j = 0; j < chunks.GetLength(1); j++)
	        {
	            for (int k = 0; k < chunks.GetLength(2); k++)
	            {
                    Transform currentChunk = Instantiate(ChunkPrefab, new Vector3(i * 16f, j * 16f, k * 16f), Quaternion.identity) as Transform;
                    currentChunk.GetComponent<Chunk>().ChunkID = new Vector3(i,j,k);
	            }
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
