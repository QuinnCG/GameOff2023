using System;
using UnityEngine;

namespace Quinn
{
	public class InputReader : MonoBehaviour
	{
		public Action OnPrimaryDown;
		public Action OnPrimaryUp;
		public Action OnRoll;
		public Action<Vector2> OnMove;

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnPrimaryDown?.Invoke();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnPrimaryUp?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftControl))
			{
				OnRoll?.Invoke();
			}

			OnMove?.Invoke(new Vector2()
			{
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			}.normalized);
		}
	}
}
