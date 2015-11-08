using UnityEngine;

namespace Helper {
	
	/// <summary>
	/// Helper class
	/// </summary>
	public static class Helper {
		public static Bounds GetObjectBounds(GameObject obj) {
			Vector3 min = new Vector3();
			Vector3 max = new Vector3();

			min = obj.GetComponent<MeshFilter> ().mesh.bounds.min;
			max = obj.GetComponent<MeshFilter> ().mesh.bounds.max;

			return new Bounds ((max+min)/2,max-min);
		}

		public static Bounds GetObjectSize(GameObject obj) {
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

			//Vector3 min = obj.transform.position;
			//Vector3 max = obj.transform.position;

			Bounds combinedBounds = new Bounds ();

			foreach (Renderer renderer in renderers) {
				combinedBounds.Encapsulate(renderer.gameObject.GetComponent<MeshFilter> ().mesh.bounds);
				/*if (renderer.bounds.min.x+obj.transform.position.x < min.x) {
					min.x = renderer.bounds.min.x;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("min.x = " + renderer.bounds.min.x.ToString());
				}
				if (renderer.bounds.max.x+obj.transform.position.x > max.x) {
					max.x = renderer.bounds.max.x;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("max.x = " + renderer.bounds.max.x.ToString());
				}
				if (renderer.bounds.min.y+obj.transform.position.y < min.y) {
					min.y = renderer.bounds.min.y;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("min.y = " + renderer.bounds.min.y.ToString());
				}
				if (renderer.bounds.max.y+obj.transform.position.y > max.y) {
					max.y = renderer.bounds.max.y;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("max.y = " + renderer.bounds.max.y.ToString());
				}
				if (renderer.bounds.min.z+obj.transform.position.z < min.z) {
					min.z = renderer.bounds.min.z;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("min.z = " + renderer.bounds.min.z.ToString());
				}
				if (renderer.bounds.max.z+obj.transform.position.z > max.z) {
					max.z = renderer.bounds.max.z;
					//Debug.Log (renderer.gameObject.name);
					//Debug.Log ("max.z = " + renderer.bounds.max.z.ToString());
				}*/
			}

			//Debug.Log (max);
			//Debug.Log (min);
			//Debug.Log (max-min);
			//Debug.Log ((max+min)/2);
			
			return combinedBounds;
		}

		public static bool PointInRect(Vector2 point, Rect rect) {
			return (point.x >= rect.xMin && point.x <= rect.xMax && point.y >= rect.yMin && point.y <= rect.yMax);
		}

		public static bool PointOutsideMaskedAreas(Vector2 point, Rect[] rects) {
			bool outside = true;

			foreach (Rect r in rects) {
				if (PointInRect(point,r)) {
					outside = false;
				}
			}

			return outside;
		}
	}
}

