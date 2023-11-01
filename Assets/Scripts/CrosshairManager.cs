using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	public class CrosshairManager : MonoBehaviour
	{
		private Transform Crosshair;

		private void Start()
		{
			var instance = Addressables.InstantiateAsync("Crosshair.prefab").WaitForCompletion();
			Crosshair = instance.transform;
		}

		private void Update()
		{
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Crosshair.position = mousePos;
		}
	}
}
