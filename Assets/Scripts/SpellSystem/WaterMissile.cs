using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Quinn.SpellSystem
{
	public class WaterMissile : MissileSpell
	{
		private VisualEffect _waterStream;

		private Vector2 _currentEnd;
		private Vector2 _velocity;

		public WaterMissile()
		{
			IsExclusive = true;
		}

		protected override void OnProjectileSpawn(Projectile projectile) { }

		public override void OnCast()
		{
			const string key = "WaterMissile.prefab";
			var instance = Addressables.InstantiateAsync(key, Player.transform.position, Quaternion.identity).WaitForCompletion();

			_waterStream = instance.GetComponent<VisualEffect>();
			_currentEnd = GetPosition();

			DOVirtual.DelayedCall(2.1f, () =>
			{
				if (instance != null)
				{
					_waterStream = null;
					Object.Destroy(instance);
				}
			});
			DOVirtual.DelayedCall(2.25f, () => OnSpellLoseExclusivity?.Invoke());
		}

		public override void OnUpdate()
		{
			if (_waterStream != null)
			{
				UpdateStream();
			}
		}

		private void UpdateStream()
		{
			_waterStream.SetVector3("Start", Player.transform.position);
			_waterStream.SetVector3("End", _currentEnd);

			_currentEnd = Vector2.SmoothDamp(_currentEnd, GetPosition(), ref _velocity, 0.3f);
		}

		private Vector2 GetPosition()
		{
			Vector2 center = CrosshairManager.Instance.CrosshairPosition;
			return center;
		}
	}
}
