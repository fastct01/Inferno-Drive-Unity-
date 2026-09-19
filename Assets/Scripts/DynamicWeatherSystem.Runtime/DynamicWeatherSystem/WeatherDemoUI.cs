using UnityEngine;

namespace DynamicWeatherSystem
{
	public class WeatherDemoUI : MonoBehaviour
	{
		private WeatherManager weatherManager;

		private WeatherStateData presetClear;

		private WeatherStateData presetRain;

		private WeatherStateData presetFog;

		private WeatherStateData presetStorm;

		private float transitionDuration;

		private GUIStyle _panelStyle;

		private GUIStyle _titleStyle;

		private GUIStyle _buttonStyle;

		private GUIStyle _buttonActiveStyle;

		private GUIStyle _labelStyle;

		private GUIStyle _sublabelStyle;

		private bool _stylesReady;

		private static readonly Color C_Clear;

		private static readonly Color C_Rain;

		private static readonly Color C_Fog;

		private static readonly Color C_Storm;

		private void Awake()
		{
		}

		private void OnGUI()
		{
		}

		private void DrawPresetButton(string label, WeatherStateData preset, Color accent)
		{
		}

		private void DrawDurationRow(float width)
		{
		}

		private void DrawStateDisplay(float width)
		{
		}

		private static void DrawSeparator(float width)
		{
		}

		private void InitStyles()
		{
		}

		private static Texture2D MakePixel(Color color)
		{
			return null;
		}
	}
}
