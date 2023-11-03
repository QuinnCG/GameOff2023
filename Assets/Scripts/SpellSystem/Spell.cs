namespace Quinn.SpellSystem
{
	public abstract class Spell
	{
		public SpellDefinition Definition { get; private set; }
		public Player Player { get; private set; }

		public Spell() { }

		public void Initialize(SpellDefinition definition, Player player)
		{
			Definition = definition;
			Player = player;
		}

		public virtual void OnCast() { }
		public virtual void OnUpdate() { }
		public virtual void OnKilled() { }
	}
}
