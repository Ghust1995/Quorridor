using System;
using UnityEngine;
using System.Collections;
using GameData;
using Logging;
using UnityEngine.Networking;

class TryNewWallMessage : MessageBase
{
    public static short Id = 1001;
    public int PosX;
    public int PosY;
    public int StartPosX;
    public int StartPosY;
    public bool IsVertical;
}

[RequireComponent(typeof(TileMap))]
public class TileMapNetwork : NetworkBehaviour {

    TileMap _tileMap;
    TileMapMouse _tileMapMouse;
    NetworkClient _mClient;
    // TODO: Better way to make messages
    public delegate void AddWallSuccessEventHandler(int posX, int posY, bool isVertical);
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
            StartCoroutine(WaitAndTryNewWall(msg.PosX, msg.PosY, msg.StartPosX, msg.StartPosY, msg.IsVertical));
        });
        _mClient.Connect("127.0.0.1", 7777);
    }

    public void MapClickDebug(object sender, MapCreateEventArgs e)
    {
        LoggerSystem.Log("New click at X=" + e.WallPos.x + " Y=" + e.WallPos.y);
    }

    public void WallSuccessDebug(int posX, int posY, bool isVertical)
    {
        LoggerSystem.Log("New wall succeeded at X=" + posX + " Y=" + posY);
    }

    public void WallFailDebug(string message)
    {
        LoggerSystem.Log(message);
    }

    [ClientCallback]
    private void SendTryNewWallMessage(object sender, MapCreateEventArgs e)
    {
        var msg = new TryNewWallMessage()
        {
            PosX = (int)e.WallPos.x,
            PosY = (int)e.WallPos.y,
            StartPosX = (int)e.PlayerPos.x,
            StartPosY = (int)e.PlayerPos.y,
            IsVertical = e.IsVertical
        };
        _mClient.Send(TryNewWallMessage.Id, msg);
    }

    private IEnumerator WaitAndTryNewWall(int posX, int posY, int startPosX, int startPosY, bool isVertical)
    {
        var timeToWait = (new Vector2(posX - startPosX, posY - startPosY)).magnitude / GlobalConstants.Wall.Speed;
        LoggerSystem.Log("Schedule new wall creation for " + timeToWait + "s from now!");
        yield return new WaitForSeconds(timeToWait);
        string errorMessage;
        if (_tileMap.Map.AddWall(posX, posY, isVertical, out errorMessage))
        {
            if (EventAddWallSuccess == null) yield break;

            EventAddWallSuccess(posX, posY, isVertical);
        }
        else
        {
            if (EventAddWallFail == null) yield break;

            EventAddWallFail(errorMessage);
        }
    }
}
