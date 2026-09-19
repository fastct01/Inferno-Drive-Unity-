using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosImpressionDataDelegateListener : IosNativeObject
	{
		private delegate void DidReceiveImpressionData(IntPtr adPtr, string impressionDataJson);

		private readonly DidReceiveImpressionData _kImpressionCallback;

		internal IosImpressionDataDelegateListener(IosNativeObject ad)
			: base(usesCallbacks: false)
		{
		}

		public override void Dispose()
		{
		}

		[PreserveSig]
		private static extern IntPtr LPMImpressionDataDelegateCreate(IntPtr adNativePtr, DidReceiveImpressionData callback);

		[PreserveSig]
		private static extern void LPMImpressionDataDelegateDestroy(IntPtr delegatePtr);

		private static void OnImpression(IntPtr adPtr, string impressionDataJson)
		{
		}
	}
}
