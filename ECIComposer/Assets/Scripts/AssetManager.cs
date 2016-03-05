using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Helper;

public class AssetManager : MonoBehaviour {

	public Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
	ObjectSelector objSelector;
	ObjectList objList;
	
	
	private Bounds totalBounds;
	public List<GameObject> targets = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		objSelector = GameObject.Find ("ObjectSelector").GetComponent ("ObjectSelector") as ObjectSelector;
		objList = GameObject.Find ("ObjectList").GetComponent ("ObjectList") as ObjectList;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void InstantiateObject (string objName) {
		Debug.Log ("Instantiate " + objName);
		GameObject go = (GameObject)GameObject.Instantiate (prefabs [objName]);
		go.transform.position = Vector3.zero;
		go.SetActive (true);
		go.name = go.name.Replace ("(Clone)", "");

		targets.Add (go);

		foreach (Transform child in go.GetComponentsInChildren<Transform>()) {
			if (child.gameObject.GetComponent<MeshFilter> () != null) {
				//Debug.Log (child.name);
				BoxCollider collider = child.gameObject.AddComponent<BoxCollider> ();
				Bounds bounds = Helper.Helper.GetObjectBounds (child.gameObject);
				//Debug.Log (bounds.size);
				collider.center = bounds.center;
				collider.size = bounds.size;
			

				//Calculates total bounds using targets list aka all gameobjects added
				bool isReset = false;
				foreach (GameObject gameObject in targets) {
					if (isReset == false) { 
						totalBounds = new Bounds(gameObject.transform.position, new Vector3(3,3,3));
						isReset = true;
					}
					totalBounds.Encapsulate(gameObject.transform.position);
					totalBounds.Encapsulate(bounds);
				} 

				//variables needed to calculate distance of camera from bounds center
				Vector3 max = totalBounds.size;
				float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
				float distance = radius / (Mathf.Sin(Camera.main.fieldOfView * Mathf.Deg2Rad / 2f));

				//points camera at center of total bounds
				Camera.main.transform.position = totalBounds.center;
				Camera.main.transform.Translate (0 ,0, -distance); 
			}
		}

		go.AddComponent<ComposerEntity> ();

		MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer renderer in renderers) {
			renderer.gameObject.AddComponent<Outliner> ();
		}
		objSelector.selectedObjects.Clear ();
		objSelector.selectedObjects.Add (go);
	}

	public void ReifyAs (string name, List<GameObject> selectedObjects) {
		float x = 0.0f, y = 0.0f, z = 0.0f;
		foreach (GameObject obj in selectedObjects) {
			x += obj.transform.position.x;
			y += obj.transform.position.y;
			z += obj.transform.position.z;
		}

		GameObject newObj = new GameObject (name);
		newObj.transform.position = new Vector3 (x / selectedObjects.Count, y / selectedObjects.Count, z / selectedObjects.Count);

		foreach (GameObject obj in selectedObjects) {
			obj.transform.parent = newObj.transform;
		}

		prefabs.Add (newObj.name, newObj);
		List<string> tempObjs = new List<string> (objList.Objects);
		tempObjs.Add(newObj.name);
		objList.Objects = tempObjs;

		newObj.AddComponent<ComposerEntity> ();
	}

	public void ExportAssets() {
	}
}
