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
			//target = targetDir * Mathf.Min(targetDst, maxDistance);
			//target += (Vector2)Player.transform.position;

			SpawnLightning(Player.transform.position, target, attach: true);
			ChainStrike(target, new List<Damage>() { closest });
		}

		private void SpawnLightning(Vector2 start, Vector2 end, bool attach = false)
		{
			var raycasts = Physics2D.LinecastAll(start, end);
			foreach (var raycast in raycasts)
			{
				if (raycast.collider.TryGetComponent(out Damage damage) && !raycast.collider.CompareTag("Player"))
				{
					//end = damage.GetComponent<Collider2D>().bounds.center;
					break;
				}
			}

			string key = "LightningMissile.prefab";
			Addressables.InstantiateAsync(key, start, Quaternion.identity).Completed += handle =>
			{
				var instance = handle.Result;
				if (instance == null) return;

				var vfx = instance.GetComponent<VisualEffect>();

				vfx.SetVector2("Target", end);
				Object.Destroy(instance, 1f);

				if (attach)
				{
					instance.transform.parent = Player.transform;
				}

				Debug.DrawLine(start, end, Color.yellow, 5f);
			};
		}

		private void ChainStrike(Vector2 origin, List<Damage> targets)
		{
			var positions = new List<Vector2>();
			var colliders = Physics2D.OverlapCircleAll(origin, 5f);

			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Damage damage)
					&& !collider.CompareTag("Player")
					&& !targets.Contains(damage))
				{
					Vector2 end = collider.GetComponent<Collider2D>().bounds.center;
					SpawnLightning(origin, end);

					targets.Add(damage);
					positions.Add(end);
				}
			}

			foreach (var pos in positions)
			{
				ChainStrike(pos, targets);
			}
		}
	}
}
