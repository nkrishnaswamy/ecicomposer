using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Helper;

public class AssetManager : MonoBehaviour {

	public Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InstantiateObject (string objName) {
		Debug.Log ("Instantiate " + objName);
		GameObject go = (GameObject)GameObject.Instantiate (prefabs [objName], new Vector3(), new Quaternion());
		BoxCollider collider = go.AddComponent<BoxCollider>();
		Bounds bounds = Helper.Helper.GetObjectBounds (go);
		Debug.Log (bounds.size);
		collider.center = new Vector3 (bounds.center.x / go.transform.lossyScale.x,
		                               bounds.center.y / go.transform.lossyScale.y,
		                               bounds.center.z / go.transform.lossyScale.z);
		collider.size = new Vector3 (bounds.size.x / go.transform.lossyScale.x,
									 bounds.size.y / go.transform.lossyScale.y,
		                             bounds.size.z / go.transform.lossyScale.z);
	}
}
