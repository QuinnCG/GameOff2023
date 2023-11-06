using System;

namespace Quinn.SpellSystem
{
	[Serializable]
	public class SpellDefinition
	{
		public float Fire;
		public float Lightning;
		public float Water;

		public float Power = 1f;
		public float Magnitude = 1f;
		public float Stability = 1f;
		public float Duplicity = 1f;

		public static SpellDefinition operator +(SpellDefinition left, SpellDefinition right)
		{
			left.Fire += right.Fire;
			left.Lightning += right.Lightning;
			left.Water += right.Water;

			left.Power += right.Power;
			left.Magnitude += right.Magnitude;
			left.Stability += right.Stability;

			return left;
		}

		public Type GetSpellType()
		{
			if (Fire > Lightning && Fire > Water)
			{
				return typeof(FireMissile);
			}
			else if (Lightning > Fire && Lightning > Water)
			{
				return typeof(LightningMissile);
			}
			else if (Water > Lightning && Water > Fire)
			{
				return typeof(WaterMissile);
			}

			throw new Exception();
		}
	}
}
