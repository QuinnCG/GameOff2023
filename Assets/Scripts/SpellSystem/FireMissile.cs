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
	}
}
