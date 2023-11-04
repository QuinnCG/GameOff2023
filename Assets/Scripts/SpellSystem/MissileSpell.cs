using UnityEngine;

namespace Quinn.SpellSystem
{
	public abstract class MissileSpell : Spell
	{
		public const float BaseProjectileSize = 0.5f;
		public const float BaseProjectileSpeed = 8f;

		protected int ProjectilesAlive { get; set; }
		protected bool KillSpellWhenNoProjectiles { get; set; } = true;

		protected virtual int GetProjectileCount()
		{
			return Mathf.RoundToInt(Definition.Duplicity);
		}

		protected virtual float GetProjectileSize()
		{
			return Definition.Magnitude / GetProjectileCount() * BaseProjectileSize;
		}

		protected virtual float GetProjectileSpeed()
		{
			return Definition.Power / (GetProjectileSize() * 1.3f) * BaseProjectileSpeed;
		}

		public override void OnCast()
		{
			SpawnProjectilesAtCrosshair(GetProjectileSettings(), GetProjectileCount());
		}

		protected virtual void OnProjectileSpawn(Projectile projectile) { }

		protected Projectile[] SpawnProjectilesAtCrosshair(ProjectileSettings settings, int count = 1)
		{
			Vector2 origin = Player.transform.position;
			Vector2 target = CrosshairManager.Instance.CrosshairPosition;
			float scale = CrosshairManager.Instance.CrosshairScale;

			var projectiles = Projectile.SpawnTargetingCircle(origin, target, scale, settings, count);
			foreach (var projectile in projectiles)
			{
				projectile.OnCollide += collider => OnProjectileCollide(projectile, collider);
				OnProjectileSpawn(projectile);

				// Count number of alive projectiles, destory spell when none are left.
				ProjectilesAlive++;

				if (KillSpellWhenNoProjectiles)
				{
					projectile.OnDestroyed += () =>
					{
						ProjectilesAlive--;
						if (ProjectilesAlive <= 0)
						{
							SpellManager.Instance.KillSpell(this);
						}
					};
				}
			}

			return projectiles;
		}

		protected virtual ProjectileSettings GetProjectileSettings()
		{
			return new ProjectileSettings()
			{
				Scale = GetProjectileSize(),
				Speed = GetProjectileSpeed()
			};
		}

		/// <returns>Whether or not the projectile is destroyed.</returns>
		protected virtual bool OnProjectileCollide(Projectile projectile, Collider2D collider)
		{
			if (collider.CompareTag("Enemy") || collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				Object.Destroy(projectile.gameObject);
				return true;
			}

			return false;
		}
	}
}
