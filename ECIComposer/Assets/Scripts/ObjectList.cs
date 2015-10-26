using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectList : MonoBehaviour {

	AssetManager assetManager;

	int bgLeft = Screen.width/48;
	int bgTop = Screen.height/24;
	int bgWidth = 9*Screen.width/48;
	int bgHeight = 10*Screen.height/12;
	
	Vector2 scrollPosition;
	
	string[] listItems;
	
	List<string> objects = new List<string>();
	
	int selected = -1;

	string objectSelected = "";
	
	GUIStyle customStyle;

	// Use this for initialization
	void Start () {
		assetManager = GameObject.Find ("AssetManager").GetComponent ("AssetManager") as AssetManager;

		foreach (KeyValuePair<string, GameObject> kv in assetManager.prefabs) {
			objects.Add(kv.Key);
		}
		
		listItems = objects.ToArray ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUILayout.BeginArea(new Rect(bgLeft, bgTop, bgWidth, bgHeight), GUI.skin.window);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false); 
		GUILayout.BeginVertical (GUI.skin.box);
		
		customStyle = GUI.skin.button;
		
		selected = GUILayout.SelectionGrid(selected, listItems, 1, customStyle, GUILayout.ExpandWidth(true));

		if (selected >= 0) {
			objectSelected = listItems [selected];
		}

		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent("Objects"));
		
		GUI.Label (new Rect (((2*bgLeft+bgWidth)/2)-textDimensions.x/2, bgTop, textDimensions.x, 25), "Objects");

		GUI.enabled = (selected != -1);
		if (GUI.Button(new Rect(bgLeft, bgTop+bgHeight+Screen.height/48, bgWidth, 25),new GUIContent("Add"))){
			assetManager.InstantiateObject(objectSelected);
		}
		GUI.enabled = true;
	}
}
