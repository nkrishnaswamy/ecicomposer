using UnityEngine;
using System.Collections;

using Helper;

public class ComposerEntity : MonoBehaviour {

	bool isSelected;
	public bool IsSelected {
		get { return isSelected; }
		set { isSelected = value; }
	}

	ObjectSelector objSelector;

	// Use this for initialization
	void Start () {
		objSelector = GameObject.Find ("ObjectSelector").GetComponent ("ObjectSelector") as ObjectSelector;
	}
	
	// Update is called once per frame
	void Update () {
		if (objSelector.selectedObjects.Contains(gameObject)) {
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
					gameObject.transform.position = new Vector3 (gameObject.transform.position.x,
					                                             gameObject.transform.position.y,
					                                             //gameObject.transform.position.z + Helper.Helper.GetObjectSize (gameObject).size.z);
					                                             gameObject.transform.position.z + 0.5f);
				}
				else {
					gameObject.transform.position = new Vector3 (gameObject.transform.position.x,
					                                             //gameObject.transform.position.y + Helper.Helper.GetObjectSize (gameObject).size.y,
					                                             gameObject.transform.position.y + 0.5f,
					                                             gameObject.transform.position.z);
				}
			}
			else
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
					gameObject.transform.position = new Vector3 (gameObject.transform.position.x,
					                                             gameObject.transform.position.y,
					                                             //gameObject.transform.position.z - Helper.Helper.GetObjectSize (gameObject).size.z);
					                                             gameObject.transform.position.z - 0.5f);
				}
				else {
					gameObject.transform.position = new Vector3 (gameObject.transform.position.x,
					                                             //gameObject.transform.position.y - Helper.Helper.GetObjectSize (gameObject).size.y,
					                                             gameObject.transform.position.y - 0.5f,
					                                             gameObject.transform.position.z);
				}
			}
			else
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				gameObject.transform.position = new Vector3 (//gameObject.transform.position.x - Helper.Helper.GetObjectSize (gameObject).size.x,
				                                             gameObject.transform.position.x - 0.5f,
				                                             gameObject.transform.position.y,
				                                             gameObject.transform.position.z);
			}
			else
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				gameObject.transform.position = new Vector3 (//gameObject.transform.position.x + Helper.Helper.GetObjectSize (gameObject).size.x,
				                                             gameObject.transform.position.x + 0.5f,
			                                            	 gameObject.transform.position.y,
				                                             gameObject.transform.position.z);
			}
		}
	}
}
