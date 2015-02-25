using UnityEngine;

namespace UnLighted
{
	[AddComponentMenu("UnLighted/Box"), RequireComponent(typeof(BoxCollider))]
	public class Box : MonoBehaviour
	{
		public Cubemap Cubemap;

		public Vector3 Size { get { return this.collider.bounds.size; } }

		public Vector3 Pos { get { return this.transform.position; } }
	}
}