using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Quinn.SpellSystem
{
	public class LightningMissile : MissileSpell
	{
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

			Vector2 target = closest != null
				? closest.GetComponent<Collider2D>().bounds.center
				: crosshairPos;

			Vector2 targetDir = (target - (Vector2)Player.transform.position).normalized;
			float targetDst = Vector2.Distance(Player.transform.position, target);

			float maxDistance = 12f;
			target = targetDir * Mathf.Min(targetDst, maxDistance);
			target += (Vector2)Player.transform.position;

			SpawnLightning(Player.transform.position, target, parent: Player.transform);
			ChainStrike(target, new List<Damage>() { closest });
		}

		private void SpawnLightning(Vector2 start, Vector2 end, Transform parent = null)
		{
			var raycasts = Physics2D.LinecastAll(start, end);
			foreach (var raycast in raycasts)
			{
				if (raycast.collider.TryGetComponent(out Damage damage)
					&& !raycast.collider.CompareTag("Player")
					&& !raycast.collider.CompareTag("Enemy")
					&& !raycast.collider.transform != parent)
				{
					end = damage.GetComponent<Collider2D>().bounds.center;
					break;
				}
			}

			// Orignally, this didn't use WaitForCompletion(). Instead if simply did all relavent setup via a callback.
			// This resulted in some weird issues with all but the first lightning strike working properly.
			string key = "LightningMissile.prefab";
			var instance = Addressables.InstantiateAsync(key, start, Quaternion.identity).WaitForCompletion();
			var vfx = instance.GetComponent<VisualEffect>();

			vfx.SetVector2("Target", end);
			Object.Destroy(instance, 1f);

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

					Debug.Log("Hit: " + collider.gameObject.name);
					Debug.Log(collider.transform.position);

					targets.Add(damage);
					positions.Add(end);
				}
			}

			// This is here for performance reasons.
			const int MaxTargetCount = int.MaxValue;
			if (targets.Count < MaxTargetCount)
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
