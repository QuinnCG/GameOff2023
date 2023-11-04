using Quinn.SpellSystem;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Caster))]
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private SpellDefinition Spell;

		private InputReader _input;
		private Movement _movement;
		private Caster _caster;

		private void Awake()
		{
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
			_caster = GetComponent<Caster>();

			_input.OnPrimaryDown += OnPrimaryDown;
			_input.OnPrimaryUp += OnPrimaryUp;
			_input.OnMove += OnMove;
		}

		private void Start()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
		}

		private void OnPrimaryDown()
		{
			CrosshairManager.Instance.ChargeCast(0.5f);
		}

		private void OnPrimaryUp()
		{
			var manager = CrosshairManager.Instance;

			if (manager.IsFullyCharged)
			{
				_caster.Cast(Spell);
			}

			manager.EndCast();
		}

		private void OnMove(Vector2 vector)
		{
			_movement.Move(vector);
		}
	}
}
