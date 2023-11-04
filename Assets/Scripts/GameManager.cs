using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("GameManager.prefab").WaitForCompletion();
			DontDestroyOnLoad(instance);
		}

		private async void Awake()
		{
			Instance = this;

			await UnityServices.InitializeAsync();
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
	}
}
