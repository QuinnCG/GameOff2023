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

			_input.OnMove += OnMove;
		}

		private void Start()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void OnMove(Vector2 vector)
		{
			_movement.Move(vector);
		}
	}
}
