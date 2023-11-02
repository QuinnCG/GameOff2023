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
		private Transform _homingTarget;

		public static void SpawnWithDir(Vector2 origin, Vector2 rawDirection, ProjectileSettings settings)
		{
			Addressables.InstantiateAsync("Projectile.prefab", origin, Quaternion.identity).Completed += handle =>
			{
				var instance = handle.Result;

				if (instance != null)
				{
					var projectile = instance.GetComponent<Projectile>();

					projectile.transform.localScale = Vector3.one * settings.Scale;
					projectile._direction = rawDirection.normalized;
					projectile.Settings = settings;
				}
			};
		}

		public static void SpawnSingle(Vector2 origin, Vector2 target, ProjectileSettings settings)
		{
			var dir = target - origin;
			dir.Normalize();

			SpawnWithDir(origin, dir, settings);
		}

		public static void SpawnShotgunSpread(Vector2 origin, Vector2 target, float spreadAngle, ProjectileSettings settings, int count = 1)
		{
			float angleDelta = spreadAngle / count;

			for (int i = 0; i < count; i++)
			{
				Vector2 dir = target - origin;
				dir.Normalize();

				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				angle += (angleDelta * i) - (spreadAngle / 2f);

				dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
				SpawnWithDir(origin, dir, settings);
			}
		}

		public static void SpawnTargetingCircle(Vector2 origin, Vector2 target, float radius, ProjectileSettings settings, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				Vector2 pos = Random.insideUnitCircle * radius / 2f;
				pos += target;

				var dir = pos - origin;
				dir.Normalize();

				SpawnWithDir(origin, dir, settings);
			}
		}

		private void Start()
		{
			_rb = GetComponent<Rigidbody2D>();
			Destroy(gameObject, Settings.MaxDistance / Settings.Speed);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				Debug.Log("Hit Obstacle!");
			}
			else if (collision.CompareTag("Enemy") && Settings.Team != Team.Enemy)
			{
				Debug.Log("Hit Enemy!");
			}
			else if (collision.CompareTag("Player") && Settings.Team != Team.Player)
			{
				Debug.Log("Hit Player!");
			}
			else if (Settings.Team == Team.None)
			{
				Debug.Log("Hit Something!");
			}
			else
			{
				return;
			}

			Destroy(gameObject);
		}

		private void Update()
		{
			if (Settings.ScaleRate != 0f)
			{
				transform.localScale += Time.deltaTime * Settings.ScaleRate * Vector3.one;
				transform.localScale = Vector3.one * Mathf.Clamp(transform.localScale.magnitude, Settings.MinScale, Settings.MaxScale);

				if (Settings.DieWhenScaleZero && transform.localScale == Vector3.zero)
				{
					Destroy(gameObject);
				}
			}
		}

		private void FixedUpdate()
		{
			if (Settings.IsHoming)
			{
				if (_homingTarget == null)
				{
					SetHomingTarget(GetHomingTarget());
				}
				else
				{
					Vector2 targetDir = (_homingTarget.position - transform.position).normalized;
					float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

					float currentAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
					float deltaAngle = currentAngle - targetAngle;

					//_direction = Quaternion.AngleAxis(deltaAngle * Time.deltaTime * Settings.HomingTurnRate, Vector3.up) * _direction;

					Debug.DrawLine(transform.position, transform.position + (Vector3)_direction, Color.red, Time.deltaTime);
					Debug.DrawLine(transform.position, transform.position + (Vector3)targetDir, Color.green, Time.deltaTime);
				}
			}

			_rb.velocity = _direction * Settings.Speed;
		}

		private Transform GetHomingTarget()
		{
			var colliders = Physics2D.OverlapCircleAll(transform.position, Settings.HomingRange);
			foreach (var collider in colliders)
			{
				if (collider.CompareTag("Player") && Settings.Team == Team.Enemy)
				{
					return collider.transform;
				}
				if (collider.CompareTag("Enemy") && Settings.Team == Team.Player)
				{
					return collider.transform;
				}
			}

			return null;
		}

		public void SetHomingTarget(Transform target)
		{
			_homingTarget = target;
		}
	}
}
