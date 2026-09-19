using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   GarageDebugger_Start_m8CC30E8603B9BEB2E3A4645639437FAB6D0DC312
//   GarageDebugger_Update_m1A0DDA945726566AA5B0D9AB4A03D30C98D27E88
public class GarageDebugger : MonoBehaviour
{
	private void Start()
	{
		Debug.Log("=== Garage Debugger Started ===");
		GameObject garageScene = Resources.Load<GameObject>("garage_scene");
		if (garageScene != null)
		{
			Debug.Log("✓ garage_scene.fbx загружена успешно");
			Debug.Log(string.Format("  - Масштаб: {0}", garageScene.transform.localScale));
			Debug.Log(string.Format("  - Позиция: {0}", garageScene.transform.localPosition));
			Debug.Log(string.Format("  - Количество детей: {0}", garageScene.transform.childCount));
		}
		else
		{
			Debug.LogError("✗ garage_scene.fbx НЕ найдена! Положите её в папку Resources/");
		}
		if (Resources.Load<GameObject>("sportcar") != null)
		{
			Debug.Log("✓ sportcar.prefab загружена успешно");
		}
		else
		{
			Debug.LogWarning("⚠ sportcar.prefab НЕ найдена (это может быть нормально)");
		}
		Camera mainCamera = Camera.main;
		Debug.Log(string.Format("✓ Main Camera активна: {0}", mainCamera != null));
		if (Object.FindObjectOfType<ShadowRunPrototype>() != null)
		{
			Debug.Log("✓ ShadowRunPrototype найден");
			return;
		}
		Debug.LogError("✗ ShadowRunPrototype НЕ найден!");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			Debug.Log("=== Garage State Info ===");
			if (Object.FindObjectOfType<ShadowRunPrototype>() != null)
			{
				Debug.Log("ShadowRunPrototype существует");
				return;
			}
			Debug.LogWarning("ShadowRunPrototype не существует");
		}
	}
}
