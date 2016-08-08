using UnityEngine;
using System.Collections;
using System;
using GameData;
using Logging;
using UnityEngine.Networking;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
    [SerializeField]
    private int _sizeX = 100;
    public int SizeX
    {
        get { return _sizeX; }
    }

    [SerializeField]
    private int _sizeY = 50;
    public int SizeY
    {
        get { return _sizeY; }
    }

    [SerializeField]
    private float _tileSize = 1.0f;
    public float TileSize
    {
        get { return _tileSize;}
    }

    [SerializeField]
    private int _wallSize = 5;
    public int WallSize
    {
        get { return _wallSize; }
    }

    public Vector3 MaptoGlobalCoords(int x, int y)
    {
        var currentGlobalPoint = new Vector3(x * TileSize, 0, y * TileSize);
        var hitPointGlobal = transform.TransformPoint(currentGlobalPoint);

        return hitPointGlobal;
    }

    public Vector2 GlobalToMapCoords(Vector3 global)
    {
        var hitPointLocal = transform.InverseTransformPoint(global);
        var currentTilePoint = new Vector2(Mathf.FloorToInt(hitPointLocal.x / TileSize), Mathf.FloorToInt(hitPointLocal.z / TileSize));

        return currentTilePoint;
    }

    
    public GridMap Map { get; private set; }

    void Awake()
    {
        Map = new GridMap(_sizeX, _sizeY, _wallSize);
    }
    
    void Start()
    {
        BuildMesh();
        BuildTexture();
    }

    void Update()
    {
        //LoggerSystem.Log(Map.ToString(), LoggerSystem.TextLogTag);
    }

    // TODO: move to builder class
    public void BuildMesh()
    {
        int numTiles = _sizeX * _sizeY;
        int numTriangles = numTiles * 2;

        var verticesSizeX = _sizeX + 1;
        var verticesSizeY = _sizeY + 1;
        int numVertices = verticesSizeX * verticesSizeY;

        // Generate the mesh data
        var vertices = new Vector3[numVertices];
        var normals = new Vector3[numVertices];
        var uv = new Vector2[numVertices];
        var triangles = new int[numTriangles * 3];

        for (int x = 0; x < verticesSizeX; x++)
        {
            for (int y = 0; y < verticesSizeY; y++)
            {
                var index = y * verticesSizeX + x;
                vertices[index] = new Vector3(x * _tileSize, 0, y * _tileSize);
                normals[index] = Vector3.up; // Probably change later;
                uv[index] = new Vector2((float)x / _sizeX, (float)y / _sizeY);
            }
        }

        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                int squareIndex = y * _sizeX + x;
                int triIndex = squareIndex * 6;
                triangles[triIndex + 0] = y * verticesSizeX + x +                 0;
                triangles[triIndex + 1] = y * verticesSizeX + x + verticesSizeX + 0;
                triangles[triIndex + 2] = y * verticesSizeX  + x + verticesSizeX + 1;

                triangles[triIndex + 3] = y * verticesSizeX + x +                 0;
                triangles[triIndex + 4] = y * verticesSizeX + x + verticesSizeX + 1;
                triangles[triIndex + 5] = y * verticesSizeX + x + 1; 
            }
        }

        // Create new mesh populated with data
        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        // Assing mesh to mesh related components
        var meshFilter = GetComponent<MeshFilter>();
        //var meshRenderer = GetComponent<MeshRenderer>();
        var meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    // TODO: move to builder class
    public void BuildTexture()
    {
        var texWidth = _sizeX;
        var texHeight = _sizeY;
        var texture = new Texture2D(texWidth, texHeight);
        for (int x = 0; x < texWidth; x++)
        {
            for (int y = 0; y < texHeight; y++)
            {
                
                if (x%2 == y%2)
                {
                    if (x <= 1)
                        texture.SetPixel(x, y, Color.green);
                    else if (x >= texWidth - 2)
                        texture.SetPixel(x, y, Color.red);
                    else
                        texture.SetPixel(x, y, Color.yellow);
                }
                else
                    texture.SetPixel(x, y, Color.black);

            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.SetTexture("_MainTex", texture);
    }
}
