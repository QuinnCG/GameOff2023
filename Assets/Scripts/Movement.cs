using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Movement : MonoBehaviour
	{
		[field: SerializeField]
		public float Speed { get; set; } = 5f;

		private Rigidbody2D _rb;
		private Vector2 _lastDir;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		public void Move(Vector2 rawDir)
		{
			_lastDir = rawDir.normalized;
		}

		private void LateUpdate()
		{
			_rb.velocity = _lastDir * Speed;
			_lastDir = Vector2.zero;
		}
	}
}
