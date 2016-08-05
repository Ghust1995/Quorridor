using System;
using UnityEngine;
using System.Collections;
using Logging;
using UnityEngine.Networking;

class TryNewWallMessage : MessageBase
{
    public static short Id = 1001;
    public int PosX;
    public int PosZ;
    public bool IsVertical;
}



[RequireComponent(typeof(TileMap))]
public class TileMapNetwork : NetworkBehaviour {

    TileMap _tileMap;
    TileMapMouse _tileMapMouse;
    NetworkClient _mClient;
    // TODO: Better way to make messages
    

    public delegate void AddWallSuccessEventHandler(int posX, int posZ, bool isVertical);
    public delegate void AddWallFailEventHandler(string message);


    [SyncEvent]
    public event AddWallSuccessEventHandler EventAddWallSuccess;

    [SyncEvent]
    public event AddWallFailEventHandler EventAddWallFail;

    void Start()
    {
        if (!isLocalPlayer)
        {
            EventAddWallSuccess += WallSuccessDebug;
            EventAddWallFail += WallFailDebug;
        }
    }

    public override void OnStartClient()
    {
        _tileMap = GetComponent<TileMap>();
        _tileMapMouse = (TileMapMouse)FindObjectOfType(typeof(TileMapMouse));
        _tileMapMouse.MapClick += SendTryNewWallMessage;
        _tileMapMouse.MapClick += MapClickDebug;
        _mClient = new NetworkClient();

        NetworkServer.RegisterHandler(TryNewWallMessage.Id, netMsg =>
        {
            var msg = netMsg.ReadMessage<TryNewWallMessage>();
            TryNewWall(msg.PosX, msg.PosZ, msg.IsVertical);
        });
        _mClient.Connect("127.0.0.1", 7777);
    }

    public void MapClickDebug(object sender, MapClickEventArgs e)
    {
        LoggerSystem.Log("New click at X=" + e.PosX + " Z=" + e.PosZ);
    }

    public void WallSuccessDebug(int posX, int posZ, bool isVertical)
    {
        LoggerSystem.Log("New wall succeeded at X=" + posX + " Z=" + posZ);
    }

    public void WallFailDebug(string message)
    {
        LoggerSystem.Log(message);
    }

    [ClientCallback]
    private void SendTryNewWallMessage(object sender, MapClickEventArgs e)
    {
        var msg = new TryNewWallMessage()
        {
            PosX = e.PosX,
            PosZ = e.PosZ,
            IsVertical = e.IsVertical
        };
        _mClient.Send(TryNewWallMessage.Id, msg);
    }

    private void TryNewWall(int posX, int posZ, bool isVertical)
    {
        LoggerSystem.Log("Command Received!");
        string errorMessage;
        if (_tileMap.Map.AddWall(posX, posZ, isVertical, out errorMessage))
        {
            if (EventAddWallSuccess == null) return;

            EventAddWallSuccess(posX, posZ, isVertical);
        }
        else
        {
            if (EventAddWallFail == null) return;

            EventAddWallFail(errorMessage);
        }
    }
}
