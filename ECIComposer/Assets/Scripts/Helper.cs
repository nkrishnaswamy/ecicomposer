using UnityEngine;

namespace Helper {
	
	/// <summary>
	/// Helper class
	/// </summary>
	public static class Helper {
		public static Bounds GetObjectBounds(GameObject obj) {
			Vector3 min = new Vector3();
			Vector3 max = new Vector3();

			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

			foreach (Renderer renderer in renderers) {
				if (renderer.bounds.min.x+renderer.gameObject.transform.position.x < min.x) {
					min.x = renderer.bounds.min.x;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("min.x = " + renderer.bounds.min.x.ToString());
				}
				if (renderer.bounds.max.x+renderer.gameObject.transform.position.x > max.x) {
					max.x = renderer.bounds.max.x;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("max.x = " + renderer.bounds.max.x.ToString());
				}
				if (renderer.bounds.min.y+renderer.gameObject.transform.position.y < min.y) {
					min.y = renderer.bounds.min.y;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("min.y = " + renderer.bounds.min.y.ToString());
				}
				if (renderer.bounds.max.y+renderer.gameObject.transform.position.y > max.y) {
					max.y = renderer.bounds.max.y;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("max.y = " + renderer.bounds.max.y.ToString());
				}
				if (renderer.bounds.min.z+renderer.gameObject.transform.position.z < min.z) {
					min.z = renderer.bounds.min.z;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("min.z = " + renderer.bounds.min.z.ToString());
				}
				if (renderer.bounds.max.z+renderer.gameObject.transform.position.z > max.z) {
					max.z = renderer.bounds.max.z;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("max.z = " + renderer.bounds.max.z.ToString());
				}
			}

			//Debug.Log (max-min);
			//Debug.Log ((max+min)/2);

			return new Bounds ((max+min)/2,max-min);
		}

		public static bool PointInRect(Vector2 point, Rect rect) {
			return (point.x >= rect.xMin && point.x <= rect.xMax && point.y >= rect.yMin && point.y <= rect.yMax);
		}
	}
}

