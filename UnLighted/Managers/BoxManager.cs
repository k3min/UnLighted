using UnityEngine;

namespace UnLighted.Managers
{
	[AddComponentMenu("UnLighted/Managers/Box")]
	public class BoxManager : ManagerBase<BoxManager>
	{
		public Box Current { get; private set; }

		private void Update()
		{
			if (Camera.main == null)
			{
				return;
			}

			Box res = null;

			var pos = Camera.main.transform.position;
			var cls = Physics.OverlapSphere(pos, QualitySettings.shadowDistance, 1 << LayerMask.NameToLayer("Box"));

			var prv = Mathf.Infinity;

			foreach (var col in cls)
			{
				var box = col.GetComponent<Box>();

				if (box == null)
				{
					continue;
				}

				var dst = col.ClosestPointOnBounds(pos) - pos;
				var mag = dst.sqrMagnitude;

				if (mag < prv)
				{
					res = box;
					prv = mag;
				}
			}

			if (res == null || res == this.Current)
			{
				return;
			}

			Shader.SetGlobalTexture("_Box", res.Cubemap);
			Shader.SetGlobalVector("_BoxSize", res.Size);
			Shader.SetGlobalVector("_BoxPos", res.Pos);

			this.Current = res;
		}
	}
}