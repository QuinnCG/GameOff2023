using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Projectile : MonoBehaviour
	{
		[SerializeField]
		private ProjectileSettings Settings;

		private Rigidbody2D _rb;
		private Vector2 _direction;

		public static void Spawn(Vector2 origin, Vector2 rawDirection, ProjectileSettings settings, float scale = 1f)
		{
			Addressables.InstantiateAsync("Projectile.prefab", origin, Quaternion.identity).Completed += handle =>
			{
				var instance = handle.Result;
				var projectile = instance.GetComponent<Projectile>();

				projectile.transform.localScale = Vector3.one * scale;
				projectile._direction = rawDirection.normalized;
				projectile.Settings = settings;
			};
		}

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Enemy"))
			{
				Debug.Log("Hit Enemy!");
			}
			else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				Debug.Log("Hit Obstacle!");
			}
		}

		private void FixedUpdate()
		{
			_rb.velocity = _direction * Settings.Speed;
		}
	}
}
