using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	public class CrosshairManager : MonoBehaviour
	{
		public static CrosshairManager Instance { get; private set; }

		[SerializeField]
		private float DefaultSize = 3f;

		public Vector2 CrosshairPosition => _crosshair.transform.position;
		public float CrosshairScale => _crosshair.transform.localScale.magnitude;
		public bool IsFullyCharged => _fullChargeTime > -1f && Time.time > _fullChargeTime;

		private Crosshair _crosshair;
		private float _fullChargeTime = -1f;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			var instance = Addressables.InstantiateAsync("Crosshair.prefab").WaitForCompletion();
			_crosshair = instance.GetComponent<Crosshair>();

			ResetRadius();
		}

		private void Update()
		{
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_crosshair.transform.position = mousePos;
		}

		public void SetRadius(float radius)
		{
			_crosshair.transform.localScale = Vector3.one * radius;
		}

		public void ResetRadius()
		{
			_crosshair.transform.localScale = Vector3.one * DefaultSize;
		}

		public void ChargeCast(float maxDuration)
		{
			_crosshair.StartCastCharge(maxDuration);
			_fullChargeTime = Time.time + maxDuration;
		}

		public void EndCast()
		{
			_crosshair.EndCastCharge();
			_fullChargeTime = -1f;
		}
	}
}
