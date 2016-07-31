using UnityEngine;
using System.Collections;

public class VoxelMap : MonoBehaviour {

    public float Size = 2f;

    public int VoxelResolution = 8;
    public int ChunkResolution = 2;

    public VoxelGrid VoxelGridPrefab;

    private VoxelGrid[] chunks;
    private float chunkSize, voxelSize, halfSize;

    private static string[] fillTypeNames = { "Filled", "Empty" };
    private static string[] radiusNames = { "0", "1", "2", "3", "4", "5" };
    private int fillTypeIndex, radiusIndex;

    private void Awake()
    {
        halfSize = Size*0.5f;
        chunkSize = Size/ChunkResolution;
        voxelSize = chunkSize/VoxelResolution;

        chunks = new VoxelGrid[ChunkResolution*ChunkResolution];

        for (int i = 0, y = 0; y < ChunkResolution; y++)
        {
            for (int x = 0; x < ChunkResolution; x++, i++)
            {
                CreateChunk(i, x, y);
            }
        }

        //add collider
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.size = new Vector3(Size,Size);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.gameObject == gameObject)
                {
                    EditVoxels(transform.InverseTransformPoint(hitInfo.point));
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(4f, 4f, 150f, 500f));
        GUILayout.Label("Fill Type");
        fillTypeIndex = GUILayout.SelectionGrid(fillTypeIndex, fillTypeNames, 2);
        GUILayout.Label("Radius");
        radiusIndex = GUILayout.SelectionGrid(radiusIndex, radiusNames, 6);
        GUILayout.EndArea();
    }

    private void EditVoxels(Vector3 point)
    {
        int centerX = (int)((point.x + halfSize) / voxelSize);
        int centerY = (int)((point.y + halfSize) / voxelSize);

        int xStart = (centerX - radiusIndex - 1) / VoxelResolution;
        if (xStart < 0)
        {
            xStart = 0;
        }
        int xEnd = (centerX + radiusIndex) / VoxelResolution;
        if (xEnd >= ChunkResolution)
        {
            xEnd = ChunkResolution - 1;
        }
        int yStart = (centerY - radiusIndex - 1) / VoxelResolution;
        if (yStart < 0)
        {
            yStart = 0;
        }
        int yEnd = (centerY + radiusIndex) / VoxelResolution;
        if (yEnd >= ChunkResolution)
        {
            yEnd = ChunkResolution - 1;
        }
        /*
        int chunkX = centerX / VoxelResolution;
        int chunkY = centerY / VoxelResolution;
        Debug.Log(centerX + ", " + centerY + " in chunk " + chunkX + ", " + chunkY);
        centerX -= chunkX * VoxelResolution;
        centerY -= chunkY * VoxelResolution;
         */
        VoxelStencil activeStencil = new VoxelStencil();
        activeStencil.Intitialize(fillTypeIndex == 0, radiusIndex);
        /*
        activeStencil.SetCenter(centerX, centerY);
        chunks[chunkY * ChunkResolution + chunkX].Apply(activeStencil);
         */
        int voxelYOffset = yEnd * VoxelResolution;
        for (int y = yEnd; y >= yStart; y--)
        {
            int i = y * ChunkResolution + xEnd;
            int voxelXOffset = xEnd * VoxelResolution;
            for (int x = xEnd; x >= xStart; x--, i--)
            {
                activeStencil.SetCenter(centerX - voxelXOffset, centerY - voxelYOffset);
                chunks[i].Apply(activeStencil);
                voxelXOffset -= VoxelResolution;
            }
            voxelYOffset -= VoxelResolution;
        }
    }


    private void CreateChunk(int i, int x, int y)
    {
        VoxelGrid chunk = Instantiate(VoxelGridPrefab) as VoxelGrid;
        chunk.Initialize(VoxelResolution, chunkSize);
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector3(x * chunkSize - halfSize, y * chunkSize - halfSize);
        chunks[i] = chunk;

        if (x > 0)
        {
            chunks[i - 1].xNeighbor = chunk;
        }
        if (y > 0)
        {
            chunks[i - ChunkResolution].yNeighbor = chunk;
            if (x > 0)
            {
                chunks[i - ChunkResolution - 1].xyNeighbor = chunk;
            }
        }
    }
}
