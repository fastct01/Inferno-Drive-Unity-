using Unity.Mathematics;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   FastSky_Sun_Color__ctor_m3A1B12725E10D4B134312F202461088881118E74
//   FastSky_Sun_Color_Update_m440E56FEE60F71FC4B64C63F03D2A90641328684
public class FastSky_Sun_Color : MonoBehaviour
{
	private Light _light;

	private float sunIntensity = 5f;

	private Color dayColour;

	private Color eveningColour;

	private void Update()
	{
		if (_light == null)
		{
			_light = GetComponent<Light>();
		}
		float elevation = Vector3.Dot(-transform.forward, Vector3.up);
		float daylight = Mathf.Clamp(elevation + 0.9f, 0f, 1f);
		float dusk = (1f - Mathf.Clamp01(elevation)) * Mathf.Clamp01(Mathf.Sign(elevation));
		float dawn = (1f - Mathf.Clamp01(-elevation)) * Mathf.Clamp01(Mathf.Sign(-elevation));
		dusk = Mathf.Pow(math.smoothstep(0f, 0.9f, dusk), 5f);
		dawn = Mathf.Pow(dawn, 5f);
		_light.intensity = Mathf.Lerp(0.1f, sunIntensity, Mathf.Pow(daylight, 5f));
		_light.color = Color.Lerp(dayColour, eveningColour, dusk + dawn);
		RenderSettings.ambientIntensity = Mathf.Lerp(2.5f, 1f, Mathf.Pow(daylight, 5f));
	}
}
