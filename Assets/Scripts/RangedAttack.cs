using UnityEngine;

namespace Quinn
{
	public class RangedAttack : MonoBehaviour
	{
		public void FireSingle(Vector2 origin, Vector2 target, ProjectileSettings settings)
		{
			var dir = target - origin;
			dir.Normalize();

			Projectile.Spawn(origin, dir, settings);
		}

		public void FireShotgunSpread(Vector2 origin, Vector2 target, float spreadAngle, ProjectileSettings settings, int count = 1)
		{
			float angleDelta = spreadAngle / count;

			for (int i = 0; i < count; i++)
			{
				Vector2 dir = target - origin;
				dir.Normalize();

				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				angle += (angleDelta * i) - (spreadAngle / 2f);

				dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
				Projectile.Spawn(origin, dir, settings);
			}	
		}

		public void FireAtCircle(Vector2 origin, Vector2 target, float radius, ProjectileSettings settings, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				Vector2 pos = Random.insideUnitCircle * radius / 2f;
				pos += target;

				var dir = pos - origin;
				dir.Normalize();

				Projectile.Spawn(origin, dir, settings);
			}
		}
	}
}
