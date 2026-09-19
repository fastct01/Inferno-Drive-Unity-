using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace ShadowRun.Services
{
	public sealed class RevenueCatPurchaseService : MonoBehaviour
	{
		[Serializable]
		private sealed class ProductConfig
		{
			public string product_id;

			public int crystals;
		}

		[Serializable]
		private sealed class RevenueCatConfig
		{
			public string apple_public_sdk_key;

			public string google_public_sdk_key;

			public ProductConfig[] products;
		}

		public sealed class PurchaseOutcome
		{
			private bool Success__BackingField;

			private bool Cancelled__BackingField;

			private string ProductId__BackingField;

			private string TransactionId__BackingField;

			private string Message__BackingField;

			public bool Success
			{
				get
				{
					return Success__BackingField;
				}
				private set
				{
					Success__BackingField = value;
				}
			}

			public bool Cancelled
			{
				get
				{
					return Cancelled__BackingField;
				}
				private set
				{
					Cancelled__BackingField = value;
				}
			}

			public string ProductId
			{
				get
				{
					return ProductId__BackingField;
				}
				private set
				{
					ProductId__BackingField = value;
				}
			}

			public string TransactionId
			{
				get
				{
					return TransactionId__BackingField;
				}
				private set
				{
					TransactionId__BackingField = value;
				}
			}

			public string Message
			{
				get
				{
					return Message__BackingField;
				}
				private set
				{
					Message__BackingField = value;
				}
			}

			public static PurchaseOutcome Completed(string productId, string transactionId)
			{
				return null;
			}

			public static PurchaseOutcome Failed(string message, bool cancelled = false)
			{
				return null;
			}
		}

		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			public static Func<ProductConfig, string> _003C_003E9__29_0;

			internal string _003CLoadProductsAsync_003Eb__29_0(ProductConfig product)
			{
				return null;
			}
		}

		private sealed class _003C_003Ec__DisplayClass25_0
		{
			public ProductConfig configured;

			public TaskCompletionSource<PurchaseOutcome> completion;

			internal void _003CPurchaseAsync_003Eb__0(Purchases.PurchaseResult result)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass28_0
		{
			public TaskCompletionSource<bool> completion;

			public RevenueCatPurchaseService _003C_003E4__this;

			public string userId;

			internal void _003CIdentifyAsync_003Eb__0(Purchases.CustomerInfo customerInfo, bool created, Purchases.Error error)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass29_0
		{
			public RevenueCatPurchaseService _003C_003E4__this;

			public TaskCompletionSource<bool> completion;

			public string[] identifiers;

			internal void _003CLoadProductsAsync_003Eb__1(List<Purchases.StoreProduct> products, Purchases.Error error)
			{
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CEnsureReadyForCurrentAccountCoreAsync_003Ed__27 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public RevenueCatPurchaseService _003C_003E4__this;

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
		private struct _003CPurchaseAsync_003Ed__25 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PurchaseOutcome> _003C_003Et__builder;

			public RevenueCatPurchaseService _003C_003E4__this;

			public int crystalAmount;

			private _003C_003Ec__DisplayClass25_0 _003C_003E8__1;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private TaskAwaiter<PurchaseOutcome> _003C_003Eu__2;

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
		private struct _003CPurchaseCrystalsAsync_003Ed__23 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PurchaseOutcome> _003C_003Et__builder;

			public int crystalAmount;

			private TaskAwaiter<PurchaseOutcome> _003C_003Eu__1;

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
		private struct _003CStart_003Ed__21 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public RevenueCatPurchaseService _003C_003E4__this;

			private YieldAwaitable.YieldAwaiter _003C_003Eu__1;

			private TaskAwaiter<bool> _003C_003Eu__2;

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

		private const string ConfigResourceName = "revenuecat";

		private static RevenueCatPurchaseService instance;

		private readonly Dictionary<int, ProductConfig> productsByAmount;

		private readonly Dictionary<string, Purchases.StoreProduct> storeProducts;

		private Purchases purchases;

		private RevenueCatConfig config;

		private Task<bool> identifyTask;

		private Task<bool> productsTask;

		private string identifiedUserId;

		private bool purchaseInProgress;

		private bool sdkStarted;

		private string configurationError;

		public static bool IsConfigured => false;

		public static string ConfigurationError => null;

		private static void CreateInstance()
		{
		}

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

		public static Task<PurchaseOutcome> PurchaseCrystalsAsync(int crystalAmount)
		{
			return null;
		}

		public static string GetLocalizedPrice(int crystalAmount, string fallback)
		{
			return null;
		}

		private Task<PurchaseOutcome> PurchaseAsync(int crystalAmount)
		{
			return null;
		}

		private Task<bool> EnsureReadyForCurrentAccountAsync()
		{
			return null;
		}

		private Task<bool> EnsureReadyForCurrentAccountCoreAsync()
		{
			return null;
		}

		private Task<bool> IdentifyAsync(string userId)
		{
			return null;
		}

		private Task<bool> LoadProductsAsync()
		{
			return null;
		}

		private void HandleAccountChanged()
		{
		}

		private void LoadConfiguration()
		{
		}

		private static bool IsPublicSdkKey(string value)
		{
			return false;
		}
	}
}
