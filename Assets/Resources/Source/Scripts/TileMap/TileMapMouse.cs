using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class MapClickEventArgs : EventArgs
{
    public int PosX { get; set;}
    public int PosZ { get; set; }
    public bool IsVertical { get; set; }
}

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TileMap))]
public class TileMapMouse : NetworkBehaviour {

    public event EventHandler<MapClickEventArgs> MapClick;

    private void OnMapClick(MapClickEventArgs e)
    {
        if (MapClick != null)
        {
            MapClick.Invoke(this, e);
        }
    }

    MeshCollider _collider;
    MeshRenderer _renderer;

    TileMap _tileMap;

    public Transform _selectionGraphic;

    private bool _isVertical = true;

    private int _curPosX = 0;
    private int _curPosZ = 0;

    void Start () {
        _collider = GetComponent<MeshCollider>();
        _renderer = GetComponent<MeshRenderer>();
        _tileMap = GetComponent<TileMap>();
    }

	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if(_collider.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            var currentTilePoint = _tileMap.GlobalToMapCoords(hitInfo.point);

            _selectionGraphic.localPosition = currentTilePoint;

            _curPosX = (int)currentTilePoint.x;
            _curPosZ = _tileMap.SizeZ - (int)currentTilePoint.z;
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

        if(Input.GetMouseButtonDown(0))
        {            
            OnMapClick(new MapClickEventArgs()
            {
                PosX = _curPosX,
                PosZ = _tileMap.SizeZ - _curPosZ,
                IsVertical = _isVertical
            });
        }

	}
}
