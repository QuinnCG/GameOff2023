using UnityEngine;

namespace Quinn
{
	public class PlayerAnimator : PlayableAnimator
	{
		[SerializeField]
		private AnimationClip Idle, Run, Roll;

		public bool IsMoving { private get; set; }

		private bool _isRolling;

		protected override void Awake()
		{
			base.Awake();
			PlayClip(Idle);
		}

		private void LateUpdate()
		{
			if (!_isRolling)
			{
				SetClip(IsMoving ? Run : Idle);
			}
		}

		public void PlayRoll()
		{
			_isRolling = true;

			PlayClip(Roll, () =>
			{
				_isRolling = false;
			});
		}
	}
}
