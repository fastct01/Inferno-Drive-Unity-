using System;
using System.Collections.Generic;

namespace Supabase.Core.Interfaces
{
	public interface IGettableHeaders
	{
		Func<Dictionary<string, string>> GetHeaders { get; set; }
	}
}
