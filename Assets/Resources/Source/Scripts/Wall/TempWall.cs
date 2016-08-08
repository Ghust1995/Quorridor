using UnityEngine;
using System.Collections;
using Logging;

public class TempWall : MonoBehaviour
{

    [SerializeField] private float _speed = 10;
    // Using future X, Y local coordinates
    private Vector2 _destination;
    private float _totalTime;
    private TileMap _tileMap;

    // Use this for initialization
    void Start () {
        _tileMap = (TileMap)FindObjectOfType(typeof(TileMap));
        if (_tileMap == null)
        {
            LoggerSystem.Log("Unable to Find TileMap");
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
	    _tileMap.GlobalToMapCoords(transform.position);

	}
}
