using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShadowRun.Services
{
	public sealed class FirstLaunchAuthScreen : MonoBehaviour
	{
		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			public static UnityAction _003C_003E9__16_4;

			public static UnityAction _003C_003E9__16_5;

			internal void _003CBuildUi_003Eb__16_4()
			{
			}

			internal void _003CBuildUi_003Eb__16_5()
			{
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CRunAppleAsync_003Ed__18 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public FirstLaunchAuthScreen _003C_003E4__this;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CRunEmailAsync_003Ed__17 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public FirstLaunchAuthScreen _003C_003E4__this;

			public bool register;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CRunGoogleAsync_003Ed__19 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public FirstLaunchAuthScreen _003C_003E4__this;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private static bool IsVisible__BackingField;

		private InputField emailInput;

		private InputField passwordInput;

		private Text statusText;

		private Text emailFormTitle;

		private Button[] actionButtons;

		private GameObject choiceRoot;

		private GameObject emailFormRoot;

		private bool emailRegisterMode;

		public static bool IsVisible
		{
			get
			{
				return IsVisible__BackingField;
			}
			private set
			{
				IsVisible__BackingField = value;
			}
		}

		private static void CreateInstance()
		{
		}

		public static void Show()
		{
		}

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		private void BuildUi()
		{
		}

		private Task RunEmailAsync(bool register)
		{
			return null;
		}

		private Task RunAppleAsync()
		{
			return null;
		}

		private Task RunGoogleAsync()
		{
			return null;
		}

		private void ShowEmailForm(bool register)
		{
		}

		private void ShowChoices()
		{
		}

		private void SetInteractable(bool value)
		{
		}

		private void SetStatus(string message)
		{
		}

		private void Close()
		{
		}

		private static Image MakeImage(Transform parent, string name, Color color)
		{
			return null;
		}

		private static Text MakeText(Transform parent, string value, int size, FontStyle style, Vector2 position, Vector2 dimensions, Color color)
		{
			return null;
		}

		private static Button MakeButton(Transform parent, string label, Vector2 position, Color background, Color? textColor = null, Vector2? size = null)
		{
			return null;
		}

		private static Button MakeArtworkButton(Transform parent, string name, string resource, Rect crop, Rect normalizedBounds)
		{
			return null;
		}

		private static Button MakeRoundedTextButton(Transform parent, string label, Rect normalizedBounds, int fontSize)
		{
			return null;
		}

		private static Button MakeLegalLinkButton(Transform parent, string label, Rect normalizedBounds, UnityAction onClick)
		{
			return null;
		}

		private static Sprite LoadFullSprite(string resource)
		{
			return null;
		}

		private static Sprite LoadTightSprite(string resource, Rect crop, bool sliced)
		{
			return null;
		}

		private static InputField MakeInput(Transform parent, string name, string placeholderValue, Rect normalizedBounds, bool password)
		{
			return null;
		}

		private static Sprite LoadBuiltinRoundedSprite()
		{
			return null;
		}

		private static Text MakeNormalizedText(Transform parent, string value, int size, FontStyle style, Rect normalizedBounds, Color color)
		{
			return null;
		}

		private static void SetNormalizedRect(RectTransform rect, Rect bounds)
		{
		}

		private static void StretchWithInset(RectTransform rect, float inset)
		{
		}

		private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 size, Vector2 position)
		{
		}

		private static void Stretch(RectTransform rect)
		{
		}

		private void _003CBuildUi_003Eb__16_0()
		{
		}

		private void _003CBuildUi_003Eb__16_1()
		{
		}

		private void _003CBuildUi_003Eb__16_2()
		{
		}

		private void _003CBuildUi_003Eb__16_3()
		{
		}

		private void _003CBuildUi_003Eb__16_6()
		{
		}
	}
}
