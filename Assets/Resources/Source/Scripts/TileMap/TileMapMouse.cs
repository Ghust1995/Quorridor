using UnityEngine;
using System.Collections;
using System;
using Logging;
using UnityEngine.Networking;

public class MapCreateEventArgs : EventArgs
{
    public Vector2 PlayerPos { get; set;}
    public Vector2 WallPos { get; set; }
    public bool IsVertical { get; set; }
}

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TileMap))]
public class TileMapMouse : NetworkBehaviour {

    public event EventHandler<MapCreateEventArgs> MapClick;

    private void OnMapClick(MapCreateEventArgs e)
    {
        if (MapClick != null)
        {
            MapClick.Invoke(this, e);
        }
    }

    private MeshCollider _collider;
    private MeshRenderer _renderer;
    private PlayerController _playerController;

    TileMap _tileMap;

    public Transform _selectionGraphic;

    private bool _isVertical = true;

    private Vector2 _curMousePos;

    void Start () {
        _collider = GetComponent<MeshCollider>();
        _renderer = GetComponent<MeshRenderer>();
        _tileMap = GetComponent<TileMap>();
    }

    public void RegisterLocalPlayer(PlayerController local)
    {
        LoggerSystem.Log("Initialized local player info on TileMap");
        local.MouseClick += OnPlayerMouseClick;
    }

    private void OnPlayerMouseClick(object sender, MouseClickEventArgs e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e.MousePos);
        RaycastHit hitInfo;
        if (_collider.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            var currentTilePoint = _tileMap.GlobalToMapCoords(hitInfo.point);
            _selectionGraphic.localPosition = new Vector3(currentTilePoint.x, 0, currentTilePoint.y);
            var wallPos = new Vector2((int)currentTilePoint.x, (int)currentTilePoint.y);

            OnMapClick(new MapCreateEventArgs()
            {
                PlayerPos = _tileMap.GlobalToMapCoords(e.PlayerPos),
                WallPos = wallPos,
                IsVertical = _isVertical
            });
        }
    }

    void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if(_collider.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            var currentTilePoint = _tileMap.GlobalToMapCoords(hitInfo.point);

            _selectionGraphic.localPosition = new Vector3(currentTilePoint.x, 0, currentTilePoint.y);

            _curMousePos = new Vector2((int) currentTilePoint.x, _tileMap.SizeY - (int) currentTilePoint.y);
        }

        if(Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < _selectionGraphic.childCount; i++)
            {
                // Very bad
                _selectionGraphic.GetChild(i).transform.Rotate(0, 90, 0);
            }
            _isVertical = !_isVertical;
        }
	}
}
