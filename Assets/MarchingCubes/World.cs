using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

    private bool[,,] worldVals = new bool[3,3,3];
    private Chunk[,,] chunks = new Chunk[3,3,3];
    public Transform ChunkPrefab;

	// Use this for initialization
	void Start () {

        //spawn chunks
	    for (int i = 0; i < chunks.GetLength(0); i++)
	    {
	        for (int j = 0; j < chunks.GetLength(1); j++)
	        {
	            for (int k = 0; k < chunks.GetLength(2); k++)
	            {
                    Transform currentChunk = Instantiate(ChunkPrefab, new Vector3(i * 4f, j * 4f, k * 4f), Quaternion.identity) as Transform;
	                Chunk chunkScript = currentChunk.GetComponent<Chunk>();
                    chunkScript.ChunkID = new Vector3(i,j,k);
	                chunks[i, j, k] = chunkScript;
	            }
	        }
	    }
        //set refs for neighbors
	    for (int x = 0; x < chunks.GetLength(0); x++)
	    {
	        for (int y = 0; y < chunks.GetLength(1); y++)
	        {
	            for (int z = 0; z < chunks.GetLength(2); z++)
	            {
	                int caseIndex = GetNeighborCaseIndex(x, y, z, chunks.GetLength(0));
	                switch (caseIndex)
	                {
                        case 0:
                            chunks[x, y, z].SetChunkRefs(caseIndex, chunks[x + 1, y, z], chunks[x, y + 1, z], chunks[x, y, z + 1], chunks[x + 1, y + 1, z], chunks[x + 1, y, z + 1], chunks[x, y + 1, z + 1], chunks[x + 1, y + 1, z + 1]);
                            break;
                        case 1:
                            chunks[x, y, z].SetChunkRefs(caseIndex, null, chunks[x, y + 1, z], chunks[x, y, z + 1], null, null, chunks[x, y + 1, z + 1], null);
	                        break;
                        case 2:
                            chunks[x, y, z].SetChunkRefs(caseIndex, chunks[x + 1, y, z], null, chunks[x, y, z + 1], null, chunks[x + 1, y, z + 1], null, null);
                            break;
                        case 3:
                            chunks[x, y, z].SetChunkRefs(caseIndex, null, null, chunks[x, y, z + 1], null, null, null, null);
                            break;
                        case 4:
                            chunks[x, y, z].SetChunkRefs(caseIndex, chunks[x + 1, y, z], chunks[x, y + 1, z], null, chunks[x + 1, y + 1, z], null, null, null);
                            break;
                        case 5:
                            chunks[x, y, z].SetChunkRefs(caseIndex, null, chunks[x, y + 1, z], null, null, null, null, null);
                            break;
                        case 6:
                            chunks[x, y, z].SetChunkRefs(caseIndex, chunks[x + 1, y, z], null, null, null, null, null, null);
                            break;
                        case 7:
                            chunks[x, y, z].SetChunkRefs(caseIndex, null, null, null, null, null, null, null);
                            break;
	                }
	            }
	        }
	    }
	}

    /// <summary>
    /// Get index of edge case to determine neighboring chunks
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private int GetNeighborCaseIndex(int x, int y, int z, int size)
    {
        bool xEdge = x == size - 1;
        bool yEdge = y == size - 1;
        bool zEdge = z == size - 1;

        //bitmask conditions to get neighbor index
        int caseIndex = 0;
        if (xEdge) { caseIndex |= 1; }
        if (yEdge) { caseIndex |= 2; }
        if (zEdge) { caseIndex |= 4; }

        //case list:
        //0 n/a
        //1 x
        //2 y
        //3 xy
        //4 z
        //5 xz
        //6 yz
        //7 xyz

        return caseIndex;
    }
}
