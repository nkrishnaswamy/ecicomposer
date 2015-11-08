using UnityEngine;
using System.Collections;

public class Outliner : MonoBehaviour {
	
	public Color meshColor = new Color(1f,1f,1f,0.5f);
	public Color outlineColor = new Color(0f,1f,0.3f,0.5f);

	GameObject highlight;

	ObjectSelector objectSelector;

	// Use this for initialization
	public void Start () {
		objectSelector = GameObject.Find ("ObjectSelector").GetComponent ("ObjectSelector") as ObjectSelector;
		
		// Set the transparent material to this object
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		Material[] materials = meshRenderer.materials;
		int materialsNum = materials.Length;
		/*for(int i = 0; i < materialsNum; i++) {
			materials[i].shader = Shader.Find("Outline/Transparent");
			materials[i].SetColor("_color", meshColor);
		}*/
		
		// Create copy of this object, this will have the shader that makes the real outline
		highlight = new GameObject ();
		highlight.transform.position = transform.position;
		highlight.transform.rotation = transform.rotation;
		highlight.transform.localScale = transform.lossyScale;
		highlight.AddComponent<MeshFilter>();
		highlight.AddComponent<MeshRenderer>();
		Mesh mesh;
		mesh = (Mesh) Instantiate(GetComponent<MeshFilter>().mesh);
		highlight.GetComponent<MeshFilter>().mesh = mesh;
		
		highlight.transform.parent = this.transform;
		materials = new Material[materialsNum];
		for(int i = 0; i < materialsNum; i++) {
			materials[i] = new Material(Shader.Find("Outline/Outline"));
			materials[i].SetColor("_OutlineColor", outlineColor);
			materials[i].SetFloat("_Outline", .005f*transform.lossyScale.x);
		}
		highlight.GetComponent<MeshRenderer>().materials = materials;
		highlight.SetActive (false);
	}

	void Update() {
		if (!highlight.activeInHierarchy) {
			if (objectSelector.selectedObjects.Contains (gameObject.transform.root.gameObject)) {
				highlight.SetActive (true);
			}
		}
		else
		if (!objectSelector.selectedObjects.Contains (gameObject.transform.root.gameObject)) {
			highlight.SetActive (false);
		}
	}
}
