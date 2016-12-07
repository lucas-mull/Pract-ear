using UnityEngine;
using System.Collections;

public class SceneStart : MonoBehaviour {

    public GameObject scene;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * 10, Space.World);
    }
}
