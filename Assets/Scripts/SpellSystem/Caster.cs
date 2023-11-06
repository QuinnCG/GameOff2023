using System;
using UnityEngine;

namespace Quinn.SpellSystem
{
	public class Caster : MonoBehaviour
	{
		public bool IsExclusiveSpellActive { get; private set; }

		public void Cast(SpellDefinition definition)
		{
			var spell = (Spell)Activator.CreateInstance(definition.GetSpellType());
			spell.Initialize(definition, GetComponent<Player>());

			SpellManager.Instance.RegisterSpell(spell);
			spell.OnCast();

			if (spell is MissileSpell missile && missile.IsExclusive)
			{
				IsExclusiveSpellActive = true;

				missile.OnSpellLoseExclusivity += () =>
				{
					IsExclusiveSpellActive = false;
				};
			}
		}
	}
}
