using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(RangedAttack))]
	public class Player : MonoBehaviour
	{
		private InputReader _input;
		private Movement _movement;
		private RangedAttack _ranged;

		private void Awake()
		{
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
			_ranged = GetComponent<RangedAttack>();

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
			CrosshairManager.Instance.ChargeCast(1f);
		}

		private void OnPrimaryUp()
		{
			var manager = CrosshairManager.Instance;

			if (manager.IsFullyCharged)
			{
				_ranged.FireAtCircle(transform.position, manager.CrosshairPosition, manager.CrosshairScale, ProjectileSettings.Default, 4);
			}

			manager.EndCast();
		}

		private void OnMove(Vector2 vector)
		{
			_movement.Move(vector);
		}
	}
}
