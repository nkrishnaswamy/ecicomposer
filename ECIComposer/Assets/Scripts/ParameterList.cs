using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParameterList : MonoBehaviour {

	AssetManager assetManager;
	
	int bgLeft = Screen.width/24;
	int bgTop = Screen.height/24;
	int bgWidth = 9*Screen.width/48;
	int bgHeight = 10*Screen.height/12;
	
	Vector2 scrollPosition;
	
	string[] listItems;
	
	List<string> objects = new List<string>();
	
	int selected = -1;
	
	GUIStyle customStyle;

	// Use this for initialization
	void Start () {
		listItems = objects.ToArray ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUILayout.BeginArea(new Rect(19*bgLeft, bgTop, bgWidth, bgHeight), GUI.skin.window);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false); 
		GUILayout.BeginVertical (GUI.skin.box);
		
		customStyle = GUI.skin.button;
		
		selected = GUILayout.SelectionGrid(selected, listItems, 1, customStyle, GUILayout.ExpandWidth(true));
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent("Parameters"));
		
		GUI.Label (new Rect (((38*bgLeft+bgWidth)/2)-textDimensions.x/2, bgTop, textDimensions.x, 25), "Parameters");

		GUI.enabled = (selected != -1);
		if (GUI.Button(new Rect(19*bgLeft, bgTop+bgHeight+Screen.height/48, bgWidth, 25),new GUIContent("Add"))){
		}
		GUI.enabled = true;
	}
}
