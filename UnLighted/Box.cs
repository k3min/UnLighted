using UnityEngine;

namespace UnLighted
{
	[AddComponentMenu("UnLighted/Box"), RequireComponent(typeof(OcclusionArea))]
	public class Box : MonoBehaviour
	{
		private OcclusionArea area;

		public Cubemap Cubemap;

		public Vector3 Size { get { return this.area.size; } }

		public Vector3 Pos { get { return this.transform.position; } }

		private void Awake()
		{
			this.area = this.GetComponent<OcclusionArea>();

			var box = this.gameObject.AddComponent<BoxCollider>();

			box.size = this.Size;
			box.isTrigger = true;
			box.hideFlags = HideFlags.HideInInspector;
		}
	}
}