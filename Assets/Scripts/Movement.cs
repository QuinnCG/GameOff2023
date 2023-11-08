using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Movement : MonoBehaviour
	{
		[field: SerializeField]
		public float Speed { get; set; } = 5f;
		
		public Vector2 Velocity => _rb.velocity;

		private Rigidbody2D _rb;
		private Vector2 _lastDir;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void LateUpdate()
		{
			_rb.velocity = _lastDir * Speed;
			_lastDir = Vector2.zero;
		}

		public void Move(Vector2 rawDir)
		{
			if (rawDir.x > 0f)
			{
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else if (rawDir.x < 0f)
			{
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}

			_lastDir = rawDir.normalized;
		}
	}
}
