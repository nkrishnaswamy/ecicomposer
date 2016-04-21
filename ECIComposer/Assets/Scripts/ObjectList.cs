using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectList : MonoBehaviour {
	//testing skins - amy
	public GUISkin customSkin;

	AssetManager assetManager;
	ParameterList parameterList;

	int bgLeft = Screen.width/48;
	int bgTop = Screen.height/24;
	int bgWidth = 9*Screen.width/48;
	int bgHeight = 37*Screen.height/48;

	public Rect bgRect;
	public Rect leftAddRect;
	public Rect rightAddRect;

	Vector2 scrollPosition;
	
	string[] listItems;
	
	List<string> objects = new List<string>();
	public List<string> Objects {
		get { return objects; }
		set {
			objects = value;
			listItems = objects.ToArray ();
		}
	}
	
	int selected = -1;

	public string objectSelected = "";
	
	GUIStyle customStyle;

	public Rect boundingBox; 

	// Use this for initialization
	void Start () {
		assetManager = GameObject.Find ("AssetManager").GetComponent ("AssetManager") as AssetManager;
		parameterList = GameObject.Find ("ParameterList").GetComponent ("ParameterList") as ParameterList;

		foreach (KeyValuePair<string, GameObject> kv in assetManager.prefabs) {
			objects.Add(kv.Key);
		}
		
		listItems = objects.ToArray ();

		bgRect = new Rect (bgLeft, bgTop, bgWidth, bgHeight);

		//left and right side Add button
		leftAddRect = new Rect(bgLeft, bgTop+bgHeight+Screen.height/48, bgWidth, 25);
		rightAddRect = new Rect (19*bgLeft, bgTop + bgHeight + Screen.height / 48, bgWidth, 25);
	}
	
	// Update is called once per frame
	void Update () {


	}

	void OnGUI () {
		//testing skins - amy
		GUI.skin = customSkin;

		GUILayout.BeginArea(bgRect, GUI.skin.window);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false); 
		GUILayout.BeginVertical (GUI.skin.box);
		
		customStyle = GUI.skin.button;
		
		selected = GUILayout.SelectionGrid(selected, listItems, 1, customStyle, GUILayout.ExpandWidth(true));

		if (selected >= 0) {
			objectSelected = listItems [selected];

			List<string> paramList = new List<string>();
			
			if (parameterList.availableParams.ContainsKey(objectSelected)) {
				paramList.AddRange(parameterList.availableParams[objectSelected]);
			}

			parameterList.listItems = paramList.ToArray();
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

		GUI.enabled = false;
		if (GUI.Button(new Rect(bgLeft, bgTop+bgHeight+(Screen.height/48)+30, bgWidth, 25),new GUIContent("Export Assets"))){
			assetManager.ExportAssets();

		}
		GUI.enabled = true;
	}
}
