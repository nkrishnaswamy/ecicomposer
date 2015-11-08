using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AssetLoader : MonoBehaviour {

	AssetManager assetManager;

	void Start() {
		assetManager = gameObject.GetComponent ("AssetManager") as AssetManager;
		StartCoroutine (DownloadAndCache());
	}

	// Use this for initialization
	IEnumerator DownloadAndCache () {

		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;

		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		//Debug.Log (Caching.CleanCache());
		using(WWW www = WWW.LoadFromCacheOrDownload ("http://dominic.cs-i.brandeis.edu/cwc/assets/demoassets", 1)){
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;

			// Load objects asynchronously
			AssetBundleRequest request = bundle.LoadAllAssetsAsync(typeof(GameObject));
			
			// Wait for completion
			yield return request;
			
			foreach (UnityEngine.Object o in request.allAssets) {
				GameObject obj = o as GameObject;
				Debug.Log(obj.name);
				AddPrefab(obj);
			}
			// Unload the AssetBundles compressed contents to conserve memory
			bundle.Unload(false);
			
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)

		// enable dependent components
		ObjectSelector objSelector = GameObject.Find ("ObjectSelector").GetComponent ("ObjectSelector") as ObjectSelector;
		objSelector.enabled = true;

		ObjectList objList = GameObject.Find ("ObjectList").GetComponent ("ObjectList") as ObjectList;
		objList.enabled = true;

		ParameterList paramList = GameObject.Find ("ParameterList").GetComponent ("ParameterList") as ParameterList;
		paramList.enabled = true;
	}

	public void AddPrefab(GameObject prefab)
	{
		assetManager.prefabs.Add(prefab.name, prefab);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
