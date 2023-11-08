using Quinn.SpellSystem;
using System;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Caster))]
	[RequireComponent(typeof(PlayerAnimator))]
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private SpellDefinition Spell;

		private InputReader _input;
		private Movement _movement;
		private Caster _caster;
		private PlayerAnimator _animator;

		private bool _isCharging;

		private void Awake()
		{
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
			_caster = GetComponent<Caster>();
			_animator = GetComponent<PlayerAnimator>();

			_input.OnPrimaryUp += OnPrimaryUp;
			_input.OnMove += OnMove;
			_input.OnRoll += OnRoll;
		}

		private void Start()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
		}

		private void Update()
		{
			if (_input.IsPrimaryDown && !_isCharging && !_caster.IsExclusiveSpellActive)
			{
				_isCharging = true;
				CrosshairManager.Instance.ChargeCast(0.5f);
			}
		}

		private void OnPrimaryUp()
		{
			var manager = CrosshairManager.Instance;

			if (manager.IsFullyCharged)
			{
				_caster.Cast(Spell);
			}

			manager.EndCast();
			_isCharging = false;
		}

		private void OnMove(Vector2 vector)
		{
			_movement.Move(vector);
			_animator.IsMoving = vector.magnitude > 0f;
		}

		private void OnRoll()
		{
			_animator.PlayRoll();
		}
	}
}
