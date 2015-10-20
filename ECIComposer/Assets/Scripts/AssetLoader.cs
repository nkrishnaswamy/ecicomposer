using UnityEngine;
using System;
using System.Collections;

public class AssetLoader : MonoBehaviour {

	void Start() {
		StartCoroutine (DownloadAndCache());
	}

	// Use this for initialization
	IEnumerator DownloadAndCache () {

		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;

		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		using(WWW www = WWW.LoadFromCacheOrDownload ("http://dominic.cs-i.brandeis.edu/cwc/assets/assets", 1)){
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
			}
			// Unload the AssetBundles compressed contents to conserve memory
			bundle.Unload(false);
			
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
