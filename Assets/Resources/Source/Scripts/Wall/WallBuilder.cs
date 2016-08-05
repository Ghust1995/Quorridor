using UnityEngine;
using System.Collections;
using GameData;
using Logging;
using UnityEngine.Networking;

public class WallBuilder : NetworkBehaviour {
    [SerializeField]
#pragma warning disable 649
    GameObject Wall;
#pragma warning restore 649

    TileMapNetwork _tileMapNetwork;
    TileMap _tileMap;
    void Start()
    {
        _tileMapNetwork = (TileMapNetwork)FindObjectOfType(typeof(TileMapNetwork));
        if (_tileMapNetwork == null)
        {
            LoggerSystem.Log("Unable to Find TileMapNetwork");
            return;
        }
        _tileMapNetwork.EventAddWallSuccess += SpawnWall;

        _tileMap = (TileMap)FindObjectOfType(typeof(TileMap));
        if (_tileMap == null)
        {
            LoggerSystem.Log("Unable to Find TileMapMouse");
            return;
        }

        //TODO: Something better for global constants.
    }

    [ClientCallback]
    void SpawnWall(int posX, int posZ, bool isVertical)
    {
        if (!isClient)
            return;
        //Checking if wall can be spawned
        //Check first if wall is overlapping the other wall;
        //string errorMessage;
        //if (_tileMap.Map.AddWall(posX, posZ, isVertical, out errorMessage))
        //{
        GameObject newWall;
        // TODO: Ajeitar Gambiarras
        if(isVertical)
            
            newWall = (GameObject)Instantiate(Wall, _tileMap.MaptoGlobalCoords(posX, posZ), Quaternion.AngleAxis(0, Vector3.up));
        else
        {
            newWall = (GameObject)Instantiate(Wall, _tileMap.MaptoGlobalCoords(posX, posZ + 1), Quaternion.AngleAxis(90, Vector3.up));
        }
        newWall.transform.parent = transform;
        //NetworkServer.Spawn(newWall);
        //};
    }
}

