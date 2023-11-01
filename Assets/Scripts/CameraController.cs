using Cinemachine;
using UnityEngine;

namespace Quinn
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private Transform Player;
		[SerializeField]
		private float CrosshairBias = 0.5f;

		private Transform _target;

		private void Start()
		{
			_target = new GameObject("Camera Target").transform;
			GetComponent<CinemachineVirtualCamera>().Follow = _target;
		}

		private void Update()
		{
			var origin = Player.position;
			var target = CrosshairManager.Instance.CrosshairPosition;

			var newPos = Vector2.Lerp(origin, target, CrosshairBias);
			_target.position = newPos;
		}
	}
}
