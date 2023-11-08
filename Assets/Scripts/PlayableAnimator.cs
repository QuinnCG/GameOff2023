using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Quinn
{
	public class PlayableAnimator : MonoBehaviour
	{
		private PlayableGraph _graph;
		private PlayableOutput _output;
		private AnimationClip _activeClip;

		protected virtual void Awake()
		{
			var animator = gameObject.AddComponent<Animator>();

			_graph = PlayableGraph.Create("Animation Graph");
			_output = AnimationPlayableOutput.Create(_graph, "Animation Output", animator);
		}

		protected virtual void OnDestroy()
		{
			_graph.Destroy();
		}

		public void SetClip(AnimationClip clip)
		{
			if (_activeClip != clip)
			{
				_activeClip = clip;

				var playableClip = AnimationClipPlayable.Create(_graph, clip);
				_output.SetSourcePlayable(playableClip);
			}
		}

		public void PlayClip(AnimationClip clip, AnimationClipEndCallback callback = default)
		{
			SetClip(clip);
			Play();

			DOVirtual.DelayedCall(clip.length - float.Epsilon, () =>
			{
				callback?.Invoke();
			});
		}

		public void Play()
		{
			_graph.Play();
		}

		public void Stop()
		{
			_graph.Stop();
		}
	}

	public delegate void AnimationClipEndCallback();
}
