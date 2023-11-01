using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	public class Player : MonoBehaviour
	{
		private InputReader _input;
		private Movement _movement;

		private void Awake()
		{
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();

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
			CrosshairManager.Instance.EndCast();
		}

		private void OnMove(Vector2 vector)
		{
			_movement.Move(vector);
		}
	}
}
