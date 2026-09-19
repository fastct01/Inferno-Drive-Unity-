using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicWeatherSystem
{
	public class WeatherManager : MonoBehaviour
	{
		private sealed class _003CTransitionRoutine_003Ed__20 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public WeatherStateData from;

			public WeatherStateData to;

			public float duration;

			public WeatherManager _003C_003E4__this;

			private float _003Celapsed_003E5__2;

			object IEnumerator<object>.Current => null;

			object IEnumerator.Current => null;

			public _003CTransitionRoutine_003Ed__20(int _003C_003E1__state)
			{
			}

			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			void IEnumerator.Reset()
			{
			}
		}

		private WeatherStateData initialState;

		private float defaultTransitionDuration;

		private AnimationCurve transitionCurve;

		private WeatherModule[] _modules;

		private WeatherStateData _currentState;

		private WeatherStateData _fromState;

		private WeatherStateData _runtimeSnapshot;

		private Coroutine _activeTransition;

		private float _transitionProgress;

		public WeatherStateData CurrentState => null;

		public bool IsTransitioning => false;

		public float TransitionProgress => 0f;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

		public void SetWeather(WeatherStateData state)
		{
		}

		public void SetWeather(WeatherStateData state, float duration)
		{
		}

		private IEnumerator TransitionRoutine(WeatherStateData from, WeatherStateData to, float duration)
		{
			return null;
		}

		private void ApplyToAll(WeatherStateData state)
		{
		}

		private void BlendAll(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private WeatherStateData CreateSnapshot(WeatherStateData from, WeatherStateData to, float t)
		{
			return null;
		}

		private static void CopyFastSkySnapshot(WeatherStateData from, WeatherStateData to, float t, WeatherStateData target)
		{
		}

		private void DestroySnapshot()
		{
		}
	}
}
