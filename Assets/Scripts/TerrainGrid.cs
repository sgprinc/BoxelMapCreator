using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGrid : MonoBehaviour {
    public GameObject nodePrefab;

    public int sizeX;
    public int sizeZ;
    public int offset = 2;
    public float amp;
    public float freq;
    public int seed;
    private float control;

    public Node[,] grid;
    public GameObject container;

    public Material Grass;
    public Material Sand;
    public Material Salt;
    public Material Earth;
    public Material Water;
    public Material Lava;

    private static TerrainGrid instance = null;
    public static TerrainGrid GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
        CreateGrid();
        CreateMouseCollider();
        control = amp + freq + seed;
    }

    private void LateUpdate() {
        float value = amp + freq + seed;
        if (freq > 0 & value != control) {
            UpdateGrid();
            control = amp + freq + seed;
        }
    }

    void CreateGrid() {
        //Create initial 2D array on X and Z
        grid = new Node[sizeX, sizeZ];

        for (int x = 0; x < sizeX; ++x) {
            float posX = (float)x * offset - ((sizeX - 1f) / 2);
            for (int z = 0; z < sizeZ; ++z) {
                float posZ = (float)z * offset - ((sizeZ - 1f) / 2);

                //float posY = 0; //Flat grid
                //float posY = Random.Range(0f, 0.2f); //Random grid

                float posY = Mathf.PerlinNoise((seed + posX + x) / freq, (posZ + z) / freq) * amp; //Perlin-Noise grid

                GameObject go = Instantiate(nodePrefab, new Vector3(posX, posY, posZ), Quaternion.Euler(90, 0, 0)) as GameObject;

                go.GetComponent<Renderer>().material = Grass;

                go.transform.parent = container.transform;
                go.transform.name = "Tile[" + x + ":" + z + "]";

                Node node = new Node();
                node.vis = go;
                node.tileRenderer = node.vis.GetComponentInChildren<MeshRenderer>();
                node.isWalkable = true;
                node.nodePosX = (int)posX;
                node.nodePosY = posY + 0.5f;
                node.nodePosZ = (int)posZ;
                grid[x, z] = node;
            }
        }
    }

    private void UpdateGrid() {
        InterfaceManager.GetInstance().AddMessageToBox("Regenerating Terrain");
        for (int x = 0; x < sizeX; ++x) {
            for (int z = 0; z < sizeZ; ++z) {
                GameObject tile = GameObject.Find("Tile[" + x + ":" + z + "]");

                float newY = Mathf.PerlinNoise((seed + tile.transform.position.x + x) / freq, (tile.transform.position.z + z) / freq) * amp; //Perlin-Noise grid

                Vector3 oldPosition = tile.transform.position;
                Vector3 newPosition = new Vector3(oldPosition.x, newY, oldPosition.z);
                tile.transform.position = newPosition;

                SceneObject placedObject = grid[x, z].placedObject;
                if (placedObject != null) {
                    placedObject.Move(newPosition);
                }
            }
        }
    }

    void CreateMouseCollider() {
        GameObject go = new GameObject("Collider");
        go.AddComponent<BoxCollider>();
        go.GetComponent<BoxCollider>().size = new Vector3(sizeX * offset, 0.1f, sizeZ * offset);
        go.transform.position = new Vector3(0, 0, 0);
        go.transform.parent = container.transform;
        go.transform.name = "Terrain Mouse Collider";
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition) {
        float worldX = worldPosition.x;
        float worldZ = worldPosition.z;

        worldX /= offset;
        worldZ /= offset;

        int x = Mathf.RoundToInt(worldX + (float)(sizeX - 1) / 2);
        int z = Mathf.RoundToInt(worldZ + (float)(sizeZ - 1) / 2);

        if (x > sizeX - 1)
            x = sizeX - 1;
        if (x < 0)
            x = 0;
        if (z > sizeZ - 1)
            z = sizeZ - 1;
        if (z < 0)
            z = 0;

        return grid[x, z];
    }

    private class GroundBlock {
        private enum BlockTypes {Grass, Ice, Water, Earth, Lava}
        private BlockTypes blockType;

        private GameObject block;

        public GroundBlock(GameObject nodePrefab, float posX, float posY, float posZ) {
            GameObject go = Instantiate(nodePrefab, new Vector3(posX, posY, posZ), Quaternion.Euler(90, 0, 0)) as GameObject;

            Material newMat = Resources.Load("Salt", typeof(Material)) as Material;
            go.GetComponent<Renderer>().material = newMat;

            this.block = go;
        }

        public GameObject getBlock() {
            return block;
        }
    } 
}
