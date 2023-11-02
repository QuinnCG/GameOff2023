using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[System.Serializable]
	public class ProjectileSettings
	{
		public static ProjectileSettings Default { get; } = new();

		public float Speed = 8f;

		[Space, FoldoutGroup("Homing Settings")]
		public bool IsHoming = false;
		[ShowIf(nameof(IsHoming)), FoldoutGroup("Homing Settings")]
		public float HomingTurnRate = 15f;
		[ShowIf(nameof(IsHoming)), FoldoutGroup("Homing Settings")]
		public float HomingRange = 3f;

		[Space, FoldoutGroup("Wavey Settings")]
		public bool IsWavey = false;
		[ShowIf(nameof(IsWavey)), FoldoutGroup("Wavey Settings")]
		public float WaveyFrequency = 1f;
		[ShowIf(nameof(IsWavey)), FoldoutGroup("Wavey Settings")]
		public float WaveyAmplitude = 1f;
	}
}
