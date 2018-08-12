using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviourSingleton<TerrainManager>
{
    public int ChunkSize;
    public Vector2Int ChunksCount;
    public int SpaceHeight;
    public int BaseTerrainHeight;
    public int MaxTerrainHeight;
    public float PerlinNoiseScale;
    public GameObject Chunk;
    public GameObject Voxel;

    private ChunkData[,] _chunks;

    public TerrainManager()
    {
    }

    private void Start()
    {
        RegenerateTerrain();
    }

    private void Update()
    {
        HandleMouseClick();
    }

    private void RegenerateTerrain()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        _chunks = new ChunkData[ChunksCount.x, ChunksCount.y];
        for (var x = 0; x < ChunksCount.x; x++)
        {
            for (var y = 0; y < ChunksCount.y; y++)
            {
                var chunk = Instantiate(Chunk, new Vector3(x * ChunkSize, 0, y * ChunkSize), Quaternion.identity, gameObject.transform);

                var chunkScript = chunk.GetComponent<ChunkData>();
                chunkScript.Generate(x, y, SpaceHeight, BaseTerrainHeight, MaxTerrainHeight, ChunkSize, PerlinNoiseScale);

                _chunks[x, y] = chunkScript;
            }
        }
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var dir = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(dir, out hit))
            {
                var hitPoint = hit.point;
                if (hitPoint.y % 1 == 0) hitPoint -= new Vector3(0, 0.5f, 0);
                if (hitPoint.x % 1 == 0 && hitPoint.x < Camera.main.transform.position.x) hitPoint -= new Vector3(0.5f, 0, 0);

                var fixedVoxelPosition = new Vector3((int)hitPoint.x, (int)hitPoint.y, (int)hitPoint.z);

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        for (int z = -2; z <= 2; z++)
                        {
                            var voxelToCheck = new Vector3(x, y, z) + fixedVoxelPosition;
                            var q = Vector3.Distance(voxelToCheck, fixedVoxelPosition);
                            //if (Vector3.Distance(voxelToCheck, fixedVoxelPosition) <= 2 && _voxels[(int)voxelToCheck.x, (int)voxelToCheck.z, (int)voxelToCheck.y] != null)
                            {
                                Explosion(hit.point, voxelToCheck, 1);
                            }
                        }
                    }
                }
                RegenerateTerrain();
            }
        }
    }

    private void Explosion(Vector3 hitPoint, Vector3 voxelPosition, int range)
    {
        //_voxels[(int)voxelPosition.x, (int)voxelPosition.z, (int)voxelPosition.y] = null;

        //var voxelAfterExplosion = Instantiate(Voxel, voxelPosition, Quaternion.identity);
        //voxelAfterExplosion.transform.GetChild(0).GetComponent<Rigidbody>().AddExplosionForce(500, hitPoint, 20, 20);
    }
}