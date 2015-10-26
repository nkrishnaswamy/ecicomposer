using UnityEngine;
using System.Collections;

public class ObjectSelector : MonoBehaviour {

	public int inspectorWidth = 200;
	public int inspectorHeight = 300;
	public int inspectorMargin = 100;

	Vector2 scrollPosition;

	Vector2 inspectorPosition;
	float inspectorPositionAdjX;
	float inspectorPositionAdjY;
	string inspectorText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
	GUIStyle inspectorStyle;
	Rect inspectorRect = new Rect(0,0,0,0);
	string inspectorObject;

	bool drawInspector;
	public bool DrawInspector {
		get { return drawInspector; }
		set {
			drawInspector = value;
			if (!drawInspector) {
				inspectorRect = new Rect(0,0,0,0);
				scrollPosition = new Vector2(0,0);
			}
		}
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			// Casts the ray and get the first game object hit
			Physics.Raycast (ray, out hit);
			if (hit.collider == null){
				//Debug.Log ("none");
				if (!Helper.Helper.PointInRect(new Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y), inspectorRect)) {
					DrawInspector = false;
				}
			}
			else{
				//Debug.Log (hit.transform.name);
				if (!Helper.Helper.PointInRect(new Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y), inspectorRect)) {
					if (hit.transform.name != inspectorObject) {
						DrawInspector = false;
					}
				}
			}
		}
		else
		if (Input.GetMouseButtonDown (1)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			// Casts the ray and get the first game object hit
			Physics.Raycast (ray, out hit);
			if (hit.collider == null){
				if (!Helper.Helper.PointInRect(new Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y), inspectorRect)) {
					DrawInspector = false;
				}
			}
			else {
				DrawInspector = true;
				scrollPosition = new Vector2(0,0);
				inspectorObject = hit.transform.name;
				inspectorPosition = new Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y);
			}
		}
	}

	void OnGUI () {
		if (DrawInspector) {
			inspectorPositionAdjX = inspectorPosition.x;
			inspectorPositionAdjY = inspectorPosition.y;
			if (inspectorPosition.x+inspectorWidth > Screen.width) {
				if (inspectorPosition.y > Screen.height-inspectorMargin) {
					inspectorPositionAdjX = inspectorPosition.x-inspectorWidth;
					inspectorPositionAdjY = inspectorPosition.y-inspectorHeight;
					inspectorRect = new Rect(inspectorPosition.x-inspectorWidth,inspectorPosition.y-inspectorHeight,inspectorWidth,inspectorHeight);
				}
				else
				if (inspectorPosition.y+inspectorHeight > Screen.height) {
					inspectorPositionAdjX = inspectorPosition.x-inspectorWidth;
					inspectorRect = new Rect(inspectorPosition.x-inspectorWidth,inspectorPosition.y,inspectorWidth,Screen.height-inspectorPosition.y);
				}
				else {
					inspectorPositionAdjX = inspectorPosition.x-inspectorWidth;
					inspectorRect = new Rect(inspectorPosition.x-inspectorWidth,inspectorPosition.y,inspectorWidth,inspectorHeight);
				}
			}
			else
			if (inspectorPosition.y > Screen.height-inspectorMargin) {
				inspectorPositionAdjY = inspectorPosition.y-inspectorHeight;
				inspectorRect = new Rect(inspectorPosition.x,inspectorPosition.y-inspectorHeight,inspectorWidth,inspectorHeight);
			}
			else
			if (inspectorPosition.y+inspectorHeight > Screen.height) {
				inspectorRect = new Rect(inspectorPosition.x,inspectorPosition.y,inspectorWidth,Screen.height-inspectorPosition.y);
			}
			else {
				inspectorRect = new Rect(inspectorPosition.x,inspectorPosition.y,inspectorWidth,inspectorHeight);
			}
			GUILayout.BeginArea(inspectorRect, GUI.skin.window);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
			inspectorStyle = GUI.skin.box;
			inspectorStyle.wordWrap = true;
			inspectorStyle.alignment = TextAnchor.MiddleLeft;
			GUILayout.BeginVertical (inspectorStyle);
			GUILayout.Box(inspectorText, GUILayout.ExpandWidth(false));
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();

			Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(inspectorObject));
			
			GUI.Label (new Rect (((2*inspectorPositionAdjX+inspectorWidth)/2)-textDimensions.x/2, inspectorPositionAdjY, textDimensions.x, 25), inspectorObject);
		}
	}
}
