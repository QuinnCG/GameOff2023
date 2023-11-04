using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Quinn.SpellSystem
{
	public class FireMissile : MissileSpell
	{
		protected override ProjectileSettings GetProjectileSettings()
		{
			var settings = base.GetProjectileSettings();
			settings.PrefabKey = "FireMissile.prefab";

			return settings;
		}

		protected override void OnProjectileSpawn(Projectile projectile)
		{
			float radius = GetProjectileSize() / 2f;

			var vfx = projectile.GetComponentInChildren<VisualEffect>();
			vfx.SetFloat("Radius", radius);

			projectile.GetComponent<CircleCollider2D>().radius = radius * 0.8f;
		}

		protected override float GetProjectileSpeed()
		{
			return base.GetProjectileSpeed() * 0.5f;
		}

		protected override float GetProjectileSize()
		{
			return base.GetProjectileSize() * 2f;
		}

		protected override bool OnProjectileCollide(Projectile projectile, Collider2D collider)
		{
			if (collider.CompareTag("Enemy") || collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				Addressables.InstantiateAsync("FireMissileExplosion.prefab", projectile.transform.position, Quaternion.identity).Completed += handle =>
				{
					Object.Destroy(handle.Result, 1.5f);
				};

				var vfx = projectile.transform.GetChild(0).parent;
				vfx.parent = null;
				Object.Destroy(vfx.gameObject, 5f);

				Object.Destroy(projectile.gameObject);

				return true;
			}

			return false;
		}
	}
}
