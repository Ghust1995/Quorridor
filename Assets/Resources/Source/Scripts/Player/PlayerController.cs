using UnityEngine;
using System.Collections;
using GameData;
using Logging;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

public class MouseClickEventArgs : EventArgs
{
    public Vector3 MousePos { get; set; }
    public Vector3 PlayerPos { get; set; }
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private float _speed;

    Rigidbody _rigidbody;

    private TileMap _tileMap;

    public char ID;

    public event EventHandler<MouseClickEventArgs> MouseClick;
    public event EventHandler RotateWall;
    public void HandleMouseClick(MouseClickEventArgs e)
    {
        if(MouseClick != null)
            MouseClick.Invoke(this, e);
    }
    public Func<bool> IsRotatingWall;

    public void HandleRotateWall()
    {
        if (RotateWall != null)
            RotateWall.Invoke(this, EventArgs.Empty);
    }


    public override void OnStartLocalPlayer()
    {
        LoggerSystem.Log("New Player Connected");
        _rigidbody = GetComponent<Rigidbody>();

        _tileMap = FindObjectOfType<TileMap>();
        if (_tileMap == null)
        {
            LoggerSystem.Log("Unable to Find TileMaps");
        }

        FindObjectOfType<TileMapMouse>().RegisterLocalPlayer(this);
    }

	public void MoveTowards(float x, float y)
    {
        if (!isLocalPlayer)
            return;
        _rigidbody.velocity = new Vector3(x, 0, y).normalized * _speed + new Vector3(0, _rigidbody.velocity.y, 0);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        var loc_pos = _tileMap.GlobalToMapCoords(transform.position);
        _tileMap.Map.UpdatePlayer((int)loc_pos.x, (int)loc_pos.y, ID);
        //foreach (var actionToEventPair in ActionsToEvents)
        //{
        //    if (actionToEventPair.Key.Invoke())
        //    {
        //        actionToEventPair.Value();
        //    }
        //}
    }

    private void OnPlayerClick()
    {
        
    }
}
