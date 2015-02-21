using UnityEngine;

namespace UnLighted
{
	[AddComponentMenu("UnLighted/Box"), RequireComponent(typeof(BoxCollider))]
	public class Box : MonoBehaviour
	{
		public Cubemap Cubemap;

		public Vector3 Size { get { return base.collider.bounds.size; } }

		public Vector3 Pos { get { return base.transform.position; } }
	}
}