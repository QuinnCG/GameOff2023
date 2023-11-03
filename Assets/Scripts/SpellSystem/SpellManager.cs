using System.Collections.Generic;
using UnityEngine;

namespace Quinn.SpellSystem
{
	public class SpellManager : MonoBehaviour
	{
		public static SpellManager Instance { get; private set; }

		private readonly HashSet<Spell> _spells = new();

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			// Since spells can call KillSpell from within the OnUpdate method,
			// this is required to avoid invalidating a foreach loop.

			var spells = new List<Spell>(_spells);
			while (spells.Count > 0)
			{
				spells[0].OnUpdate();
				spells.RemoveAt(0);
			}
		}

		public void RegisterSpell(Spell spell)
		{
			_spells.Add(spell);
		}

		public void KillSpell(Spell spell)
		{
			spell.OnKilled();
			_spells.Remove(spell);
		}
	}
}
