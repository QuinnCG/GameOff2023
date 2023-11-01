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

		public static void Spawn(Vector2 origin, Vector2 rawDirection, ProjectileSettings settings = default)
		{
			Addressables.InstantiateAsync("Projectile.prefab").Completed += handle =>
			{
				var instance = handle.Result;
				var projectile = instance.GetComponent<Projectile>();

				instance.transform.position = origin;

				projectile._direction = rawDirection.normalized;
				projectile.Settings = settings;
			};
		}

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			_rb.velocity = _direction * Settings.Speed;
		}
	}
}
