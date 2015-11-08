using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParameterList : MonoBehaviour {

	public Dictionary<string, List<string>> availableParams = new Dictionary<string, List<string>>();

	AssetManager assetManager;
	ObjectList objectList;
	
	int bgLeft = Screen.width/24;
	int bgTop = Screen.height/24;
	int bgWidth = 9*Screen.width/48;
	int bgHeight = 10*Screen.height/12;

	public Rect bgRect;
	
	Vector2 scrollPosition;
	
	public string[] listItems;
	
	//List<string> objects = new List<string>();
	
	int selected = -1;

	string paramSelected = "";

	GUIStyle customStyle;

	// Use this for initialization
	void Start () {
		assetManager = GameObject.Find ("AssetManager").GetComponent ("AssetManager") as AssetManager;
		objectList = GameObject.Find ("ObjectList").GetComponent ("ObjectList") as ObjectList;

		//listItems = objects.ToArray ();

		bgRect = new Rect (19 * bgLeft, bgTop, bgWidth, bgHeight);

		TextAsset verbList = Resources.Load ("collocations") as TextAsset;
		string[] fLines = System.Text.RegularExpressions.Regex.Split (verbList.text, "\n|\r|\r\n");
		
		for (int i = 0; i < fLines.Length; i++) {
			if (fLines[i].Length > 0) {
				string objName = fLines[i].Split(new char[]{'{'})[0].Trim();
				if (assetManager.prefabs.ContainsKey(objName)) {
					List<string> toAdd = new List<string>();
					string[] verbs = fLines[i].Split(new char[]{'{'})[1].Split(new char[]{'}'})[0].Split (new char[]{','});
					foreach (string verb in verbs) {
						string token = verb.Split (new char[]{':'})[0];

						if (token.Contains("\"")) {
							token = token.Replace("\"","");
						}
						else {
							token = token.Replace("\'","");
						}
						toAdd.Add (token);
					}
					availableParams.Add (objName,toAdd);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUILayout.BeginArea(bgRect, GUI.skin.window);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false); 
		GUILayout.BeginVertical (GUI.skin.box);
		
		customStyle = GUI.skin.button;
		
		selected = GUILayout.SelectionGrid(selected, listItems, 1, customStyle, GUILayout.ExpandWidth(true));

		if (selected >= 0) {
			paramSelected = listItems [selected];
		}

		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent("Parameters"));
		
		GUI.Label (new Rect (((38*bgLeft+bgWidth)/2)-textDimensions.x/2, bgTop, textDimensions.x, 25), "Parameters");

		if (availableParams.ContainsKey (objectList.objectSelected)) {

		}

		GUI.enabled = (selected != -1);
		if (GUI.Button(new Rect(19*bgLeft, bgTop+bgHeight+Screen.height/48, bgWidth, 25),new GUIContent("Add"))){
		}
		GUI.enabled = true;
	}
}
