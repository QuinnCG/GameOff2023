using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Projectile : MonoBehaviour
	{
		public System.Action<Collider2D> OnCollide;
		public System.Action OnDestroyed;

		private Rigidbody2D _rb;
		private Vector2 _direction;
		private Transform _homingTarget;

		private ProjectileSettings _settings;

		public static Projectile SpawnWithDir(Vector2 origin, Vector2 rawDirection, ProjectileSettings settings)
		{
			var instance = Addressables.InstantiateAsync(settings.PrefabKey, origin, Quaternion.identity).WaitForCompletion();
			var projectile = instance.GetComponent<Projectile>();

			if (instance != null)
			{
				projectile.transform.localScale = Vector3.one * settings.Scale;
				projectile._direction = rawDirection.normalized;
				projectile._settings = settings;
			}

			return projectile;
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

		public static Projectile[] SpawnTargetingCircle(Vector2 origin, Vector2 target, float radius, ProjectileSettings settings, int count = 1)
		{
			var projectiles = new Projectile[count];

			for (int i = 0; i < count; i++)
			{
				Vector2 pos = Random.insideUnitCircle * radius / 2f;
				pos += target;

				var dir = pos - origin;
				dir.Normalize();

				var proj = SpawnWithDir(origin, dir, settings);
				projectiles[i] = proj;
			}

			return projectiles;
		}

		private void Start()
		{
			_rb = GetComponent<Rigidbody2D>();
			Destroy(gameObject, _settings.MaxDistance / _settings.Speed);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			OnCollide?.Invoke(collision);
			
			//if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			//{
			//	Debug.Log("Hit Obstacle!");
			//}
			//else if (collision.CompareTag("Enemy") && Settings.Team != Team.Enemy)
			//{
			//	Debug.Log("Hit Enemy!");
			//}
			//else if (collision.CompareTag("Player") && Settings.Team != Team.Player)
			//{
			//	Debug.Log("Hit Player!");
			//}
			//else if (Settings.Team == Team.None)
			//{
			//	Debug.Log("Hit Something!");
			//}
			//else
			//{
			//	return;
			//}

			//Destroy(gameObject);
		}

		private void Update()
		{
			if (_settings.ScaleRate != 0f)
			{
				transform.localScale += Time.deltaTime * _settings.ScaleRate * Vector3.one;
				transform.localScale = Vector3.one * Mathf.Clamp(transform.localScale.magnitude, _settings.MinScale, _settings.MaxScale);

				if (_settings.DieWhenScaleZero && transform.localScale == Vector3.zero)
				{
					Destroy(gameObject);
				}
			}
		}

		private void FixedUpdate()
		{
			if (_settings.IsHoming)
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

			_rb.velocity = _direction * _settings.Speed;
		}

		private void OnDestroy()
		{
			OnDestroyed?.Invoke();
		}

		private Transform GetHomingTarget()
		{
			var colliders = Physics2D.OverlapCircleAll(transform.position, _settings.HomingRange);
			foreach (var collider in colliders)
			{
				if (collider.CompareTag("Player") && _settings.Team == Team.Enemy)
				{
					return collider.transform;
				}
				if (collider.CompareTag("Enemy") && _settings.Team == Team.Player)
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
