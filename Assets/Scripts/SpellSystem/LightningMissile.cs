using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Quinn.SpellSystem
{
	public class LightningMissile : MissileSpell
	{
		public LightningMissile()
		{
			IsExclusive = true;
		}

		protected override int GetProjectileCount()
		{
			return Mathf.RoundToInt(base.GetProjectileCount() * 2f);
		}

		protected float GetChainStrikeRadius()
		{
			return GetProjectileSize() * 2f;
		}

		protected float GetMaxDistance()
		{
			return (GetProjectileSize() * 2f) + 12f;
		}

		protected override void OnProjectileSpawn(Projectile projectile) { }

		public override void OnCast()
		{
			var crosshair = CrosshairManager.Instance;
			Vector2 crosshairPos = crosshair.CrosshairPosition;
			float crosshairRadius = crosshair.CrosshairScale;

			var colliders = Physics2D.OverlapCircleAll(crosshairPos, crosshairRadius);
			Damage closest = null;
			float distance = float.PositiveInfinity;

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Damage damage) && !collider.CompareTag("Player"))
				{
					float dst = Vector2.Distance(collider.transform.position, crosshairPos);
					if (dst < distance)
					{
						distance = dst;
						closest = damage;
					}
				}
			}

			float maxDistance = GetMaxDistance();

			Vector2 target = closest != null && distance < maxDistance
				? closest.GetComponent<Collider2D>().bounds.center
				: crosshairPos;

			Vector2 targetDir = (target - (Vector2)Player.transform.position).normalized;
			float targetDst = Vector2.Distance(Player.transform.position, target);

			target = targetDir * Mathf.Min(targetDst, maxDistance);
			target += (Vector2)Player.transform.position;

			SpawnLightning(Player.transform.position, target, parent: Player.transform);

			if (closest != null && distance <= maxDistance)
			{
				ChainStrike(target, new List<Damage>() { closest });
			}
		}

		private void SpawnLightning(Vector2 start, Vector2 end, Transform parent = null)
		{
			const string key = "LightningMissile.prefab";
			bool hitTarget = false;

			var raycasts = Physics2D.LinecastAll(start, end);
			foreach (var raycast in raycasts)
			{
				if (raycast.collider.TryGetComponent(out Damage damage)
					&& !raycast.collider.CompareTag("Player")
					&& !raycast.collider.CompareTag("Enemy")
					&& !raycast.collider.transform != parent)
				{
					end = damage.GetComponent<Collider2D>().bounds.center;
					hitTarget = true;
					break;
				}
			}

			if (hitTarget && Definition.Power > 2.5f)
			{
				Vector2 pos = end + (Vector2.up * 7f);
				var strike = Addressables.InstantiateAsync(key, pos, Quaternion.identity).WaitForCompletion();
				strike.GetComponent<VisualEffect>().SetVector2("Target", end);

				Object.Destroy(strike, 1f);
			}

			// Orignally, this didn't use WaitForCompletion(). Instead if simply did all relavent setup via a callback.
			// This resulted in some weird issues with all but the first lightning strike working properly.
			var instance = Addressables.InstantiateAsync(key, start, Quaternion.identity).WaitForCompletion();
			var vfx = instance.GetComponent<VisualEffect>();

			vfx.SetFloat("Width", GetProjectileSize() * 2f);

			vfx.SetVector2("Target", end);
			Object.Destroy(instance, 1f);

			DOVirtual.DelayedCall(1f, () => OnSpellLoseExclusivity?.Invoke());

			if (parent != null)
			{
				instance.transform.parent = parent;
			}
		}

		private void ChainStrike(Vector2 origin, List<Damage> targets)
		{
			var positions = new List<Vector2>();
			var colliders = Physics2D.OverlapCircleAll(origin, 2f);

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Damage damage)
					&& !targets.Contains(damage)
					&& !collider.CompareTag("Player"))
				{
					Vector2 end = collider.GetComponent<Collider2D>().bounds.center;
					SpawnLightning(origin, end, parent: damage.transform);

					targets.Add(damage);
					positions.Add(end);
				}
			}

			if (targets.Count < GetProjectileCount())
			{
				// The follow up chain strikes must be kept seperate from the above for loop.
				// This is because the "targets" list should be fully updated before calling any more chain strikes.
				foreach (var pos in positions)
				{
					ChainStrike(pos, targets);
				}
			}
		}
	}
}
