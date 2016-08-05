using UnityEngine;
using System.Collections;
using GameData;
using Logging;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private float _speed;

    Rigidbody _rigidbody;

    private TileMap _tileMap;

    public char ID;

    public override void OnStartLocalPlayer()
    {
        LoggerSystem.Log("New Player Connected");
        _rigidbody = GetComponent<Rigidbody>();

        _tileMap = (TileMap)FindObjectOfType(typeof(TileMap));
        if (_tileMap == null)
        {
            LoggerSystem.Log("Unable to Find TileMapMouse");
        }
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
        _tileMap.Map.UpdatePlayer((int)loc_pos.x, (int)loc_pos.z, ID);
    }
}
