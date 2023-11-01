using DG.Tweening;
using UnityEngine;

namespace Quinn
{
	public class Crosshair : MonoBehaviour
    {
		[SerializeField]
		private Transform Actor;
		[SerializeField]
		private float ShrinkDuration = 0.2f;

		private void Start()
		{
			Actor = transform.GetChild(0);
		}

		public void StartCastCharge(float maxDuration)
		{
			Actor.DOKill();
			Actor.localScale = Vector3.zero;

			Actor.DOScale(1f, maxDuration).SetEase(Ease.OutSine, 0.1f);
		}

		public void EndCastCharge()
		{
			Actor.DOPause();
			Actor.DOScale(0f, ShrinkDuration);
		}
	}
}
