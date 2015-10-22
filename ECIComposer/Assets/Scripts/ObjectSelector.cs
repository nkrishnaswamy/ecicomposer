using UnityEngine;
using System.Collections;

public class ObjectSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//AssetLoader assetLoader = GameObject.Find ("AssetLoader").GetComponent("AssetLoader") as AssetLoader;
		
		//Instantiate(assetLoader.prefabs["ball"], new Vector3(2,0,-2), new Quaternion(0,0,0,0));
		//Instantiate(assetLoader.prefabs["block"], new Vector3(-2,0,-2), new Quaternion(0,0,0,0));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			// Casts the ray and get the first game object hit
			Physics.Raycast (ray, out hit);
			if(hit.collider==null){
				Debug.Log ("none");
			}else{
				Debug.Log (hit.transform.name);
			}
		}
	}

}
