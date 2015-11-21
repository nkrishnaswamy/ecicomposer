using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ObjectSelector : MonoBehaviour {
	public List<GameObject> selectedObjects = new List<GameObject>();

	Inspector inspector;
	ObjectList objectList;
	ParameterList parameterList;

	private float distance;
	private RaycastHit selectRayhit;
	private RaycastHit dragRayhit;
	private bool lockObj;
	private GameObject collideObj;
	private Vector3 posObj;
	private Vector3 offset;
	private Vector3 screenPoint;
	// Use this for initialization
	void Start () {
		inspector = GameObject.Find ("Inspector").GetComponent ("Inspector") as Inspector;
		objectList = GameObject.Find ("ObjectList").GetComponent ("ObjectList") as ObjectList;
		parameterList = GameObject.Find ("ParameterList").GetComponent ("ParameterList") as ParameterList;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if (Helper.Helper.PointOutsideMaskedAreas (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y), 
			                                          new Rect[]{inspector.InspectorRect,objectList.bgRect,parameterList.bgRect})) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				// Casts the ray and get the first game object hit
				Physics.Raycast (ray, out selectRayhit);
				if (selectRayhit.collider == null) {
					if (!Input.GetKey (KeyCode.LeftShift) && !Input.GetKey (KeyCode.RightShift)) {
						if (!Helper.Helper.PointInRect (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y), inspector.InspectorRect)) {
							foreach (GameObject obj in selectedObjects) {
								obj.GetComponent<ComposerEntity> ().IsSelected = false;
							}
							selectedObjects.Clear ();
						}
					}

					if (!Helper.Helper.PointInRect (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y), inspector.InspectorRect)) {
						inspector.DrawInspector = false;
					}
				} else {
					if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
						if (selectedObjects.Contains (selectRayhit.transform.root.gameObject)) {
							selectRayhit.transform.root.gameObject.GetComponent<ComposerEntity> ().IsSelected = false;
							selectedObjects.Remove (selectRayhit.transform.root.gameObject);
						} else {
							selectRayhit.transform.root.gameObject.GetComponent<ComposerEntity> ().IsSelected = true;
							selectedObjects.Add (selectRayhit.transform.root.gameObject);
						}
					} else {
						foreach (GameObject obj in selectedObjects) {
							obj.GetComponent<ComposerEntity> ().IsSelected = false;
						}
						selectedObjects.Clear ();

						selectRayhit.transform.root.gameObject.GetComponent<ComposerEntity> ().IsSelected = true;
						selectedObjects.Add (selectRayhit.transform.root.gameObject);
						//Debug.Log (selectedObjects.Count);
					}

					if (!Helper.Helper.PointInRect (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y), inspector.InspectorRect)) {
						if (selectRayhit.transform.root.gameObject != inspector.InspectorObject) {
							inspector.DrawInspector = false;
						}
					}
				}
			}
		}
		else
		if (Input.GetMouseButtonDown (1)) {
			if (Helper.Helper.PointOutsideMaskedAreas (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y), 
			                                          new Rect[]{inspector.InspectorRect,objectList.bgRect,parameterList.bgRect})) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				// Casts the ray and get the first game object hit
				Physics.Raycast (ray, out selectRayhit);
				if (selectRayhit.collider == null) {
					if (!Helper.Helper.PointInRect (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y), inspector.InspectorRect)) {
						inspector.DrawInspector = false;
					}
				} else {
					inspector.DrawInspector = true;
					inspector.ScrollPosition = new Vector2 (0, 0);
					inspector.State = Inspector.InspectorState.Main;
					inspector.InspectorChoice = -1;
					inspector.InspectorObject = selectRayhit.transform.root.gameObject;
					inspector.InspectorPosition = new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y);
				}
			}
		}

		if (Input.GetMouseButton(0)) {
			if (selectRayhit.collider != null) {
				var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				var hit = Physics.Raycast (ray.origin, ray.direction, out dragRayhit);

				if (hit && !lockObj) {
					collideObj = dragRayhit.collider.gameObject;
					distance = dragRayhit.distance;
					//Debug.Log (collideObj.name);
				}

				lockObj = true;
				posObj = ray.origin + distance * ray.direction;
				screenPoint = Camera.main.WorldToScreenPoint(collideObj.transform.position);
				offset = posObj - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
				collideObj.transform.position = new Vector3 (posObj.x, posObj.y, posObj.z) - offset;
			}
		}
		else {
			lockObj = false;
		}
	}
}
