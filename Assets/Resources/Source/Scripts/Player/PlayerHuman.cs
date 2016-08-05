using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class PlayerHuman : MonoBehaviour {

    PlayerController playerController;

    // Use this for initialization
	void Start () {
        playerController = GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        playerController.MoveTowards(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
