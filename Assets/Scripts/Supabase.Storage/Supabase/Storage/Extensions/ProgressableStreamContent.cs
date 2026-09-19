using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Supabase.Storage.Interfaces;

namespace Supabase.Storage.Extensions
{
	internal class ProgressableStreamContent : HttpContent, IProgressableStreamContent
	{
		public enum UploadState
		{
			PendingUpload = 0,
			InProgress = 1,
			PendingResponse = 2,
			Complete = 3
		}

		private sealed class _003C_003Ec__DisplayClass11_0
		{
			public ProgressableStreamContent _003C_003E4__this;

			public Stream stream;

			internal void _003CSerializeToStreamAsync_003Eb__0()
			{
			}
		}

		private const int defaultBufferSize = 4096;

		private Stream content;

		private int bufferSize;

		public EventHandler<UploadState> StateChanged;

		private IProgress<float> Progress__BackingField;

		public IProgress<float> Progress
		{
			get
			{
				return Progress__BackingField;
			}
			private set
			{
				Progress__BackingField = value;
			}
		}

		public ProgressableStreamContent(Stream content)
		{
		}

		public ProgressableStreamContent(Stream content, int bufferSize, Progress<float> progress = null)
		{
		}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			return null;
		}

		protected override bool TryComputeLength(out long length)
		{
			length = default;
			return false;
		}
	}
}
