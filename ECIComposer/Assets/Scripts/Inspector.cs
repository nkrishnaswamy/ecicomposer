using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Inspector : MonoBehaviour {
	public enum InspectorState {
		Main,
		Reify,
		Markup,
		Modify,
		Delete
	};
	
	public enum InspectorMainOptions {
		Reify,
		Markup,
		Modify,
		Delete
	};
	
	public enum InspectorReifyOptions {
		Create,
		Cancel
	};
	
	public enum InspectorMarkupOptions {
		Save
	};
	
	public enum InspectorModifyOptions {
		Apply
	};
	
	public int inspectorWidth = 200;
	public int inspectorHeight = 300;
	public int inspectorMargin = 150;
	
	InspectorState inspectorState;
	public InspectorState State {
		get { return inspectorState; }
		set { inspectorState = value; }
	}
	
	AssetManager assetManager;
	ObjectSelector objectSelector;
	ObjectList objectList;
	ParameterList parameterList;
	
	Vector2 scrollPosition;
	public Vector2 ScrollPosition {
		get { return scrollPosition; }
		set { scrollPosition = value; }
	}
	
	Rect inspectorRect = new Rect(0,0,0,0);
	public Rect InspectorRect {
		get { return inspectorRect; }
		set { inspectorRect = value; }
	}
	
	Vector2 inspectorPosition;
	public Vector2 InspectorPosition {
		get { return inspectorPosition; }
		set { inspectorPosition = value; }
	}

	float inspectorPositionAdjX;
	float inspectorPositionAdjY;
	GUIStyle inspectorStyle;
	string[] inspectorMenuItems = {"Reify As...", "View/Edit Markup", "Modify", "Delete"};

	int inspectorChoice = -1;
	public int InspectorChoice {
		get { return inspectorChoice; }
		set { inspectorChoice = value; }
	}

	GameObject inspectorObject;
	public GameObject InspectorObject {
		get { return inspectorObject; }
		set { inspectorObject = value; }
	}
	
	string newName = "";
	string xScale = "1", yScale = "1", zScale = "1";

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
	
	GUIStyle listStyle = new GUIStyle ();
	Texture2D tex;
	Color[] colors;
	
	// Markup vars
	// LEX
	string mlPred = "";
	
	string[] mlTypeOptions = new string[]{"physobj","human","artifact"/*,"organic"*/};
	List<int> mlTypeSelectVisible = new List<int>(new int[]{-1});
	List<int> mlTypeSelected = new List<int>(new int[]{-1});
	int mlAddType = -1;
	List<int> mlRemoveType = new List<int>(new int[]{-1});
	int mlTypeCount = 1;
	List<string> mlTypes = new List<string>(new string[]{""});
	
	
	// TYPE
	string[] mlHeadOptions = new string[]{"cylindroid", "ellipsoid", "rectangular_prism", "toroid", "pyramidoid", "sheet"};
	int mlHeadSelectVisible = -1;
	int mlHeadSelected = -1;
	string mlHead = "";
	
	int mlAddComponent = -1;
	List<int> mlRemoveComponent = new List<int>();
	int mlComponentCount = 0;
	List<string> mlComponents = new List<string>();
	
	string[] mlConcavityOptions = new string[]{"Concave","Flat","Convex"};
	int mlConcavitySelectVisible = -1;
	int mlConcavitySelected = -1;
	string mlConcavity = "";
	
	bool mlRotatSymX = false;
	bool mlRotatSymY = false;
	bool mlRotatSymZ = false;
	bool mlReflSymXY = false;
	bool mlReflSymXZ = false;
	bool mlReflSymYZ = false;
	
	// HABITAT
	int mlAddIntrHabitat = -1;
	List<int> mlRemoveIntrHabitat = new List<int>();
	int mlIntrHabitatCount = 0;
	List<string> mlIntrHabitats = new List<string> ();
	
	int mlAddExtrHabitat = -1;
	List<int> mlRemoveExtrHabitat = new List<int>();
	int mlExtrHabitatCount = 0;
	List<string> mlExtrHabitats = new List<string> ();
	
	// AFFORD_STR
	int mlAddAffordance = -1;
	List<int> mlRemoveAffordance = new List<int>();
	int mlAffordanceCount = 0;
	List<string> mlAffordances = new List<string>();
	
	// EMBODIMENT
	string[] mlScaleOptions = new string[]{"<agent","agent",">agent"};
	int mlScaleSelectVisible = -1;
	int mlScaleSelected = -1;
	string mlScale = "";
	
	bool mlMovable = false;
	
	bool markupCleared = false;
	Voxeme loadedObject = new Voxeme();

	// Use this for initialization
	void Start () {
		assetManager = GameObject.Find ("AssetManager").GetComponent ("AssetManager") as AssetManager;
		objectSelector = GameObject.Find ("ObjectSelector").GetComponent ("ObjectSelector") as ObjectSelector;
		objectList = GameObject.Find ("ObjectList").GetComponent ("ObjectList") as ObjectList;
		parameterList = GameObject.Find ("ParameterList").GetComponent ("ParameterList") as ParameterList;
		
		colors = new Color[]{Color.white,Color.white,Color.white,Color.white};
		tex = new Texture2D (2, 2);
		
		// Make a GUIStyle that has a solid white hover/onHover background to indicate highlighted items
		listStyle.normal.textColor = Color.white;
		tex.SetPixels(colors);
		tex.Apply();
		listStyle.hover.background = tex;
		listStyle.onHover.background = tex;
		listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		if (DrawInspector) {
			inspectorPositionAdjX = inspectorPosition.x;
			inspectorPositionAdjY = inspectorPosition.y;
			if (inspectorPosition.x + inspectorWidth > Screen.width) {
				if (inspectorPosition.y > Screen.height - inspectorMargin) {
					inspectorPositionAdjX = inspectorPosition.x - inspectorWidth;
					inspectorPositionAdjY = inspectorPosition.y - inspectorHeight;
					inspectorRect = new Rect (inspectorPosition.x - inspectorWidth, inspectorPosition.y - inspectorHeight, inspectorWidth, inspectorHeight);
				} else
				if (inspectorPosition.y + inspectorHeight > Screen.height) {
					inspectorPositionAdjX = inspectorPosition.x - inspectorWidth;
					inspectorRect = new Rect (inspectorPosition.x - inspectorWidth, inspectorPosition.y, inspectorWidth, Screen.height - inspectorPosition.y);
				} else {
					inspectorPositionAdjX = inspectorPosition.x - inspectorWidth;
					inspectorRect = new Rect (inspectorPosition.x - inspectorWidth, inspectorPosition.y, inspectorWidth, inspectorHeight);
				}
			} else
			if (inspectorPosition.y > Screen.height - inspectorMargin) {
				inspectorPositionAdjY = inspectorPosition.y - inspectorHeight;
				inspectorRect = new Rect (inspectorPosition.x, inspectorPosition.y - inspectorHeight, inspectorWidth, inspectorHeight);
			} else
			if (inspectorPosition.y + inspectorHeight > Screen.height) {
				inspectorRect = new Rect (inspectorPosition.x, inspectorPosition.y, inspectorWidth, Screen.height - inspectorPosition.y);
			} else {
				inspectorRect = new Rect (inspectorPosition.x, inspectorPosition.y, inspectorWidth, inspectorHeight);
			}
			GUILayout.BeginArea (inspectorRect, GUI.skin.window);
			scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, false);
			
			if (inspectorState == InspectorState.Main) {
				GUILayout.BeginVertical (GUI.skin.box);
				inspectorStyle = GUI.skin.button;
				inspectorChoice = GUILayout.SelectionGrid(inspectorChoice, inspectorMenuItems, 1, inspectorStyle, GUILayout.ExpandWidth(true));
				GUILayout.EndVertical();
				
				if (inspectorChoice == (int)InspectorMainOptions.Reify) {	// Reify
					inspectorChoice = -1;
					inspectorState = InspectorState.Reify;
				}
				else
				if (inspectorChoice == (int)InspectorMainOptions.Markup) {	// View/Edit Markup
					inspectorChoice = -1;
					inspectorState = InspectorState.Markup;
				}
				else
				if (inspectorChoice == (int)InspectorMainOptions.Modify) {	// Modify
					inspectorChoice = -1;
					inspectorState = InspectorState.Modify;
				}
				else
				if (inspectorChoice == (int)InspectorMainOptions.Delete) {	// Delete
					inspectorChoice = -1;
					inspectorState = InspectorState.Delete;
				}
			}
			else
			if (inspectorState == InspectorState.Reify) {
				inspectorStyle = GUI.skin.box;
				GUILayout.BeginVertical (inspectorStyle);
				newName = GUILayout.TextField (newName, 25);
				inspectorChoice = GUILayout.SelectionGrid(inspectorChoice, new string[]{"Create","Cancel"}, 2, GUI.skin.button, GUILayout.ExpandWidth(true));
				GUILayout.EndVertical ();
				
				if (inspectorChoice == (int)InspectorReifyOptions.Create) {	// Create
					if (newName != "") {
						assetManager.ReifyAs(newName,objectSelector.selectedObjects);
						inspectorChoice = -1;
						newName = "";
						DrawInspector = false;
					}
				}
				else
				if (inspectorChoice == (int)InspectorReifyOptions.Cancel) {	// Cancel
					inspectorChoice = -1;
					newName = "";
					DrawInspector = false;
				}
			}
			else
			if (inspectorState == InspectorState.Markup) {
				if (File.Exists(inspectorObject.name + ".xml")) {
					if (!ObjectLoaded(inspectorObject)) {
						loadedObject = LoadMarkup(inspectorObject);
						markupCleared = false;
					}
				}
				else {
					if (!markupCleared) {
						InitNewMarkup();
						loadedObject = new Voxeme();
					}
				}
				
				inspectorStyle = GUI.skin.box;
				inspectorStyle.wordWrap = true;
				inspectorStyle.alignment = TextAnchor.MiddleLeft;
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("LEX");
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("Pred");
				mlPred = GUILayout.TextField (mlPred, 25, GUILayout.Width(100));
				GUILayout.EndHorizontal ();
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.BeginHorizontal ();
				GUILayout.Label("Type");
				
				GUILayout.BeginVertical ();
				for (int i = 0; i < mlTypeCount; i++) {
					GUILayout.BeginHorizontal ();
					if (mlTypeSelectVisible[i] == 0) {
						GUILayout.BeginVertical (inspectorStyle);
						mlTypeSelected[i] = -1;
						mlTypeSelected[i] = GUILayout.SelectionGrid(mlTypeSelected[i], mlTypeOptions, 1, listStyle, GUILayout.Width(70), GUILayout.ExpandWidth(true));
						if (mlTypeSelected[i] != -1) {
							mlTypes[i] = mlTypeOptions[mlTypeSelected[i]];
							mlTypeSelectVisible[i] = -1;
						}
						GUILayout.EndVertical ();
					}
					else {
						mlTypeSelectVisible[i] = GUILayout.SelectionGrid(mlTypeSelectVisible[i], new string[]{mlTypes[i]}, 1, GUI.skin.button, GUILayout.Width(70), GUILayout.ExpandWidth(true));
					}
					if (i != 0) { // can't remove first type
						mlRemoveType[i] = GUILayout.SelectionGrid(mlRemoveType[i], new string[]{"-"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndVertical ();
				
				GUILayout.EndHorizontal ();
				
				mlAddType = GUILayout.SelectionGrid(mlAddType, new string[]{"+"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
				
				if (mlAddType == 0) {	// add new type
					mlTypeCount++;
					mlTypes.Add ("");
					mlTypeSelectVisible.Add (-1);
					mlTypeSelected.Add (-1);
					mlRemoveType.Add(-1);
					mlAddType = -1;
				}
				
				for (int i = 0; i < mlTypeCount; i++) {
					if (mlRemoveType[i] == 0) {
						mlRemoveType[i] = -1;
						mlTypes.RemoveAt(i);
						mlRemoveType.RemoveAt(i);
						mlTypeCount--;
					}
				}
				GUILayout.EndVertical ();
				GUILayout.EndVertical ();
				
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("TYPE");
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("Head");
				
				if (mlHeadSelectVisible == 0) {
					GUILayout.BeginVertical (inspectorStyle);
					mlHeadSelected = -1;
					mlHeadSelected = GUILayout.SelectionGrid(mlHeadSelected, mlHeadOptions, 1, listStyle, GUILayout.Width(70), GUILayout.ExpandWidth(true));
					if (mlHeadSelected != -1) {
						mlHead = mlHeadOptions[mlHeadSelected];
						mlHeadSelectVisible = -1;
					}
					GUILayout.EndVertical ();
				}
				else {
					mlHeadSelectVisible = GUILayout.SelectionGrid(mlHeadSelectVisible, new string[]{mlHead}, 1, GUI.skin.button, GUILayout.Width(70), GUILayout.ExpandWidth(true));
				}
				
				GUILayout.EndHorizontal ();
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.BeginHorizontal ();
				GUILayout.Label("Components");
				mlAddComponent = GUILayout.SelectionGrid(mlAddComponent, new string[]{"+"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal ();
				
				if (mlAddComponent == 0) {	// add new component
					mlComponentCount++;
					mlComponents.Add ("");
					mlAddComponent = -1;
				}
				
				for (int i = 0; i < mlComponentCount; i++) {
					GUILayout.BeginHorizontal ();
					mlComponents[i] = GUILayout.TextField (mlComponents[i], 25, GUILayout.Width(115));
					mlRemoveComponent.Add (-1);
					mlRemoveComponent[i] = GUILayout.SelectionGrid(mlRemoveComponent[i], new string[]{"-"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal ();
				}
				
				for (int i = 0; i < mlComponentCount; i++) {
					if (mlRemoveComponent[i] == 0) {
						mlRemoveComponent[i] = -1;
						mlComponents.RemoveAt(i);
						mlRemoveComponent.RemoveAt(i);
						mlComponentCount--;
					}
				}
				
				GUILayout.EndVertical();
				
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("Concavity");
				
				if (mlConcavitySelectVisible == 0) {
					GUILayout.BeginVertical (inspectorStyle);
					mlConcavitySelected = -1;
					mlConcavitySelected = GUILayout.SelectionGrid(mlConcavitySelected, mlConcavityOptions, 1, listStyle, GUILayout.Width(70), GUILayout.ExpandWidth(true));
					if (mlConcavitySelected != -1) {
						mlConcavity = mlConcavityOptions[mlConcavitySelected];
						mlConcavitySelectVisible = -1;
					}
					GUILayout.EndVertical ();
				}
				else {
					mlConcavitySelectVisible = GUILayout.SelectionGrid(mlConcavitySelectVisible, new string[]{mlConcavity}, 1, GUI.skin.button, GUILayout.Width(70), GUILayout.ExpandWidth(true));
				}
				
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("Rotational Symmetry");
				GUILayout.BeginHorizontal ();
				mlRotatSymX = GUILayout.Toggle(mlRotatSymX,"X");
				mlRotatSymY = GUILayout.Toggle(mlRotatSymY,"Y");
				mlRotatSymZ = GUILayout.Toggle(mlRotatSymZ,"Z");
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
				
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("Reflectional Symmetry");
				GUILayout.BeginHorizontal ();
				mlReflSymXY = GUILayout.Toggle(mlReflSymXY,"XY");
				mlReflSymXZ = GUILayout.Toggle(mlReflSymXZ,"XZ");
				mlReflSymYZ = GUILayout.Toggle(mlReflSymYZ,"YZ");
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
				GUILayout.EndVertical ();
				
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("HABITAT");
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.BeginHorizontal ();
				GUILayout.Label("Intrinsic");
				
				mlAddIntrHabitat = GUILayout.SelectionGrid(mlAddIntrHabitat, new string[]{"+"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
				
				if (mlAddIntrHabitat == 0) {	// add new intrinsic habitat formula
					mlIntrHabitatCount++;
					mlIntrHabitats.Add ("Name=Formula");
					mlAddIntrHabitat = -1;
				}
				
				GUILayout.EndHorizontal ();
				
				for (int i = 0; i < mlIntrHabitatCount; i++) {
					GUILayout.BeginHorizontal ();
					mlIntrHabitats[i] = GUILayout.TextField (mlIntrHabitats[i].Split(new char[]{'='})[0], 25, GUILayout.Width(50)) + "=" +
						GUILayout.TextField (mlIntrHabitats[i].Split(new char[]{'='})[1], 25, GUILayout.Width(60));
					mlRemoveIntrHabitat.Add (-1);
					mlRemoveIntrHabitat[i] = GUILayout.SelectionGrid(mlRemoveIntrHabitat[i], new string[]{"-"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal ();
				}
				
				for (int i = 0; i < mlIntrHabitatCount; i++) {
					if (mlRemoveIntrHabitat[i] == 0) {
						mlRemoveIntrHabitat[i] = -1;
						mlIntrHabitats.RemoveAt(i);
						mlRemoveIntrHabitat.RemoveAt(i);
						mlIntrHabitatCount--;
					}
				}
				
				GUILayout.EndVertical ();
				
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.BeginHorizontal ();
				GUILayout.Label("Extrinsic");
				
				mlAddExtrHabitat = GUILayout.SelectionGrid(mlAddExtrHabitat, new string[]{"+"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
				
				if (mlAddExtrHabitat == 0) {	// add new extrinsic habitat formula
					mlExtrHabitatCount++;
					mlExtrHabitats.Add ("Name=Formula");
					mlAddExtrHabitat = -1;
				}
				
				GUILayout.EndHorizontal ();
				
				for (int i = 0; i < mlExtrHabitatCount; i++) {
					GUILayout.BeginHorizontal ();
					mlExtrHabitats[i] = GUILayout.TextField (mlExtrHabitats[i].Split(new char[]{'='})[0], 25, GUILayout.Width(50)) + "=" +
						GUILayout.TextField (mlExtrHabitats[i].Split(new char[]{'='})[1], 25, GUILayout.Width(60));
					mlRemoveExtrHabitat.Add (-1);
					mlRemoveExtrHabitat[i] = GUILayout.SelectionGrid(mlRemoveExtrHabitat[i], new string[]{"-"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal ();
				}
				
				for (int i = 0; i < mlExtrHabitatCount; i++) {
					if (mlRemoveExtrHabitat[i] == 0) {
						mlRemoveExtrHabitat[i] = -1;
						mlExtrHabitats.RemoveAt(i);
						mlRemoveExtrHabitat.RemoveAt(i);
						mlExtrHabitatCount--;
					}
				}
				
				GUILayout.EndVertical ();
				GUILayout.EndVertical ();
				
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("AFFORD_STR");
				
				GUILayout.BeginVertical (inspectorStyle);
				
				for (int i = 0; i < mlAffordanceCount; i++) {
					GUILayout.BeginHorizontal ();
					mlAffordances[i] = GUILayout.TextField (mlAffordances[i], 25, GUILayout.Width(115));
					mlRemoveAffordance.Add (-1);
					mlRemoveAffordance[i] = GUILayout.SelectionGrid(mlRemoveAffordance[i], new string[]{"-"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal ();
				}
				
				for (int i = 0; i < mlAffordanceCount; i++) {
					if (mlRemoveAffordance[i] == 0) {
						mlRemoveAffordance[i] = -1;
						mlAffordances.RemoveAt(i);
						mlRemoveAffordance.RemoveAt(i);
						mlAffordanceCount--;
					}
				}
				
				mlAddAffordance = GUILayout.SelectionGrid(mlAddAffordance, new string[]{"+"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
				
				if (mlAddAffordance == 0) {	// add new affordance
					mlAffordanceCount++;
					mlAffordances.Add ("");
					mlAddAffordance = -1;
				}
				
				GUILayout.EndVertical ();
				GUILayout.EndVertical ();
				
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("EMBODIMENT");
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("Scale");
				
				if (mlScaleSelectVisible == 0) {
					GUILayout.BeginVertical (inspectorStyle);
					mlScaleSelected = -1;
					mlScaleSelected = GUILayout.SelectionGrid(mlScaleSelected, mlScaleOptions, 1, listStyle, GUILayout.Width(70), GUILayout.ExpandWidth(true));
					if (mlScaleSelected != -1) {
						mlScale = mlScaleOptions[mlScaleSelected];
						mlScaleSelectVisible = -1;
					}
					GUILayout.EndVertical ();
				}
				else {
					mlScaleSelectVisible = GUILayout.SelectionGrid(mlScaleSelectVisible, new string[]{mlScale}, 1, GUI.skin.button, GUILayout.Width(70), GUILayout.ExpandWidth(true));
				}
				
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("Movable");
				mlMovable = GUILayout.Toggle(mlMovable,"");
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
				
				GUILayout.BeginVertical (GUI.skin.box);
				inspectorStyle = GUI.skin.button;
				inspectorChoice = GUILayout.SelectionGrid(inspectorChoice, new string[]{"Save"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
				GUILayout.EndVertical ();
				
				if (inspectorChoice == (int)InspectorMarkupOptions.Save) {	// Save
					inspectorChoice = -1;
					SaveMarkup(inspectorObject);
					DrawInspector = false;
				}
			}
			else
			if (inspectorState == InspectorState.Modify) {
				inspectorStyle = GUI.skin.box;
				GUILayout.BeginVertical (inspectorStyle);
				GUILayout.Label("Scale");
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("X");
				xScale = GUILayout.TextField (xScale, 25, GUILayout.Width(28));
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("Y");
				yScale = GUILayout.TextField (yScale, 25, GUILayout.Width(28));
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal (inspectorStyle);
				GUILayout.Label("Z");
				zScale = GUILayout.TextField (zScale, 25, GUILayout.Width(28));
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
				inspectorChoice = GUILayout.SelectionGrid(inspectorChoice, new string[]{"Apply"}, 1, GUI.skin.button, GUILayout.ExpandWidth(true));
				GUILayout.EndVertical ();
				
				if (inspectorChoice == (int)InspectorModifyOptions.Apply) {	// Apply
					inspectorObject.transform.localScale = new Vector3(inspectorObject.transform.localScale.x*System.Convert.ToSingle(xScale),
					                                                   inspectorObject.transform.localScale.y*System.Convert.ToSingle(yScale),
					                                                   inspectorObject.transform.localScale.z*System.Convert.ToSingle(zScale));
					xScale = "1";
					yScale = "1";
					zScale = "1";
					DrawInspector = false;
				}
			}
			else
			if (inspectorState == InspectorState.Delete) {
				objectSelector.selectedObjects.Remove(inspectorObject);
				inspectorObject.SetActive(false);
				inspectorObject = null;
				DrawInspector = false;
			}

			GUILayout.EndScrollView ();
			GUILayout.EndArea ();
			
			if (inspectorState == InspectorState.Markup) {
				Vector2 textDimensions = GUI.skin.label.CalcSize (new GUIContent (inspectorObject.name));
				GUI.Label (new Rect (((2 * inspectorPositionAdjX + inspectorWidth) / 2) - textDimensions.x / 2, inspectorPositionAdjY, textDimensions.x, 25), inspectorObject.name);
			}
		}
	}
	
	void InitNewMarkup() {
		// LEX
		mlPred = "";
		
		mlTypeSelectVisible = new List<int>(new int[]{-1});
		mlTypeSelected = new List<int>(new int[]{-1});
		mlAddType = -1;
		mlRemoveType = new List<int>(new int[]{-1});
		mlTypeCount = 1;
		mlTypes = new List<string>(new string[]{""});
		
		// TYPE
		mlHeadSelectVisible = -1;
		mlHeadSelected = -1;
		mlHead = "";
		
		mlAddComponent = -1;
		mlRemoveComponent = new List<int>();
		mlComponentCount = 0;
		mlComponents = new List<string>();
		
		mlConcavitySelectVisible = -1;
		mlConcavitySelected = -1;
		mlConcavity = "";
		
		mlRotatSymX = false;
		mlRotatSymY = false;
		mlRotatSymZ = false;
		mlReflSymXY = false;
		mlReflSymXZ = false;
		mlReflSymYZ = false;
		
		// HABITAT
		mlAddIntrHabitat = -1;
		mlRemoveIntrHabitat = new List<int>();
		mlIntrHabitatCount = 0;
		mlIntrHabitats = new List<string> ();
		
		mlAddExtrHabitat = -1;
		mlRemoveExtrHabitat = new List<int>();
		mlExtrHabitatCount = 0;
		mlExtrHabitats = new List<string> ();
		
		// AFFORD_STR
		mlAddAffordance = -1;
		mlRemoveAffordance = new List<int>();
		mlAffordanceCount = 0;
		mlAffordances = new List<string>();
		
		// EMBODIMENT
		mlScaleSelectVisible = -1;
		mlScaleSelected = -1;
		mlScale = "";
		
		mlMovable = false;
		
		markupCleared = true;
	}
	
	void SaveMarkup(GameObject obj) {
		Voxeme voxml = new Voxeme ();
		
		// assign VoxML values
		// PRED
		voxml.Lex.Pred = mlPred;
		voxml.Lex.Type = System.String.Join("*",mlTypes.ToArray());
		
		// TYPE
		voxml.Type.Head = mlHead;
		for (int i = 0; i < mlComponentCount; i++) {
			voxml.Type.Components.Add (new Component());
			voxml.Type.Components[i].Value = mlComponents[i];
		}
		voxml.Type.Concavity = mlConcavity;
		
		List<string> rotatSyms = new List<string> ();
		if (mlRotatSymX) {
			rotatSyms.Add ("X");
		}
		if (mlRotatSymY) {
			rotatSyms.Add ("Y");
		}
		if (mlRotatSymZ) {
			rotatSyms.Add ("Z");
		}
		voxml.Type.RotatSym = System.String.Join(",", rotatSyms.ToArray());
		
		List<string> reflSyms = new List<string> ();
		if (mlReflSymXY) {
			reflSyms.Add ("XY");
		}
		if (mlReflSymXZ) {
			reflSyms.Add ("XZ");
		}
		if (mlReflSymYZ) {
			reflSyms.Add ("YZ");
		}
		voxml.Type.ReflSym = System.String.Join(",", reflSyms.ToArray());
		
		// HABITAT
		for (int i = 0; i < mlIntrHabitatCount; i++) {
			voxml.Habitat.Intrinsic.Add (new Intr());
			voxml.Habitat.Intrinsic[i].Name = mlIntrHabitats[i].Split(new char[]{'='})[0];
			voxml.Habitat.Intrinsic[i].Value = mlIntrHabitats[i].Split(new char[]{'='})[1];
		}
		for (int i = 0; i < mlExtrHabitatCount; i++) {
			voxml.Habitat.Extrinsic.Add (new Extr());
			voxml.Habitat.Extrinsic[i].Name = mlExtrHabitats[i].Split(new char[]{'='})[0];
			voxml.Habitat.Extrinsic[i].Value = mlExtrHabitats[i].Split(new char[]{'='})[1];
		}
		
		// AFFORD_STR
		for (int i = 0; i < mlAffordanceCount; i++) {
			voxml.Afford_Str.Affordances.Add (new Affordance());
			voxml.Afford_Str.Affordances[i].Formula = mlAffordances[i];
		}
		
		// EMBODIMENT
		voxml.Embodiment.Scale = mlScale;
		voxml.Embodiment.Movable = mlMovable;
		
		voxml.Save (obj.name+".xml");
	}
	
	Voxeme LoadMarkup(GameObject obj) {
		Voxeme voxml = new Voxeme();
		string text = "";

		try {
			/*using(WWW www = WWW("http://www.voxicon.net/cwc/voxml/" + obj.name + ".xml")){
				yield return www;
				if (www.error != null)
					throw new Exception("WWW download had an error:" + www.error);
				text = www.text;
			}*/

			if (text.Length == 0) {
				voxml = Voxeme.Load (obj.name + ".xml");
			}
			
			// assign VoxML values
			// PRED
			mlPred = voxml.Lex.Pred;
			mlTypes = new List<string>(voxml.Lex.Type.Split (new char[]{'*'}));
			mlTypeCount = mlTypes.Count;
			mlTypeSelectVisible = new List<int>(new int[]{-1});
			mlTypeSelected = new List<int>(new int[]{-1});
			mlRemoveType = new List<int>(new int[]{-1});
			for (int i = 0; i < mlTypeCount; i++) {
				mlTypeSelectVisible.Add (-1);
				mlTypeSelected.Add (-1);
				mlRemoveType.Add (-1);
			}
			
			// TYPE
			mlHead = voxml.Type.Head;
			mlComponents = new List<string>();
			foreach (Component c in voxml.Type.Components) {
				mlComponents.Add (c.Value);
			}
			mlComponentCount = mlComponents.Count;
			mlConcavity = voxml.Type.Concavity;
			
			List <string> rotatSyms = new List<string>(voxml.Type.RotatSym.Split (new char[]{','}));
			mlRotatSymX = (rotatSyms.Contains("X"));
			mlRotatSymY = (rotatSyms.Contains("Y"));
			mlRotatSymZ = (rotatSyms.Contains("Z"));
			
			List<string> reflSyms = new List<string>(voxml.Type.ReflSym.Split (new char[]{','}));
			mlReflSymXY = (reflSyms.Contains ("XY"));
			mlReflSymXZ = (reflSyms.Contains ("XZ"));
			mlReflSymYZ = (reflSyms.Contains ("YZ"));
			
			// HABITAT
			mlIntrHabitats = new List<string>();
			foreach (Intr i in voxml.Habitat.Intrinsic) {
				mlIntrHabitats.Add (i.Name + "=" + i.Value);
			}
			mlIntrHabitatCount = mlIntrHabitats.Count;
			mlExtrHabitats = new List<string>();
			foreach (Extr e in voxml.Habitat.Extrinsic) {
				mlExtrHabitats.Add (e.Name + "=" + e.Value);
			}
			mlExtrHabitatCount = mlExtrHabitats.Count;
			
			// AFFORD_STR
			mlAffordances = new List<string>();
			foreach (Affordance a in voxml.Afford_Str.Affordances) {
				mlAffordances.Add (a.Formula);
			}
			mlAffordanceCount = mlAffordances.Count;
			
			// EMBODIMENT
			mlScale = voxml.Embodiment.Scale;
			mlMovable = voxml.Embodiment.Movable;
		}
		catch (FileNotFoundException ex) {
		}
		
		return voxml;
	}
	
	bool ObjectLoaded(GameObject obj) {
		bool r = false;
		
		try {
			r = ((Voxeme.Load (obj.name + ".xml")).Lex.Pred == loadedObject.Lex.Pred);
		}
		catch (FileNotFoundException ex) {
		}
		
		return r;
	}
}
