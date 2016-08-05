using UnityEngine;
using System.Collections;

public class WallTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
        BuildTexture();
    }

    public void BuildTexture()
    {
        var meshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshRenderer.material.color = UnityEngine.Random.ColorHSV();
    }
}
