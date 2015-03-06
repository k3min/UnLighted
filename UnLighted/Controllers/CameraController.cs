using UnityEngine;

namespace UnLighted.Controllers
{
	[AddComponentMenu("UnLighted/Controllers/Camera Controller"), RequireComponent(typeof(Camera))]
	public class CameraController : MonoBehaviour
	{
		private Vector2 input;
		private Vector2 rotation;
		private Vector3 rotationV;

		public float Sensitivity = 5;
		public float Smoothing = 0.04f;
		public float Tilt = -0.005f;

		private void Update()
		{
			if (Screen.lockCursor)
			{
				this.input.x += Input.GetAxis("Mouse X") * this.Sensitivity;
				this.input.y -= Input.GetAxis("Mouse Y") * this.Sensitivity;
			}

			this.input.y = Mathf.Clamp(this.input.y, -90, 90);
			this.rotation = Vector3.SmoothDamp(this.rotation, this.input, ref this.rotationV, this.Smoothing);

			var zero = Vector3.zero;

			zero.x = this.rotation.y;
			zero.y = this.rotation.x;
			zero.z = this.rotationV.x * this.Tilt;

			this.transform.localEulerAngles = zero;
		}
	}
}
