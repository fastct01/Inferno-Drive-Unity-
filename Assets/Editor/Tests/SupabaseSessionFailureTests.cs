using System.Reflection;
using NUnit.Framework;
using ShadowRun.Services;

// [user request 2026-09-27: cannot buy cars] The purchase path (RequireOnlineEconomyAsync ->
// EnsureFreshSessionAsync -> RefreshSessionCoreAsync) splits a failed token refresh two ways:
// a permanent failure asks the player to sign in, anything else is treated as a network blip and
// retried later. GoTrue's "Session expired" / refresh_token_already_used answers were missing from
// the permanent list, so a dead session was retried on every tap and the garage silently did
// nothing, forever. These tests pin the classification of the real GoTrue strings.
public class SupabaseSessionFailureTests
{
	private static bool IsPermanent(string error)
	{
		MethodInfo method = typeof(SupabaseGameManager).GetMethod("IsPermanentSessionFailure", BindingFlags.NonPublic | BindingFlags.Static);
		Assert.IsNotNull(method, "IsPermanentSessionFailure was renamed or removed");
		return (bool)method.Invoke(null, new object[] { error });
	}

	[Test]
	public void DeadSessionAnswersArePermanent()
	{
		// As GoTrue words them, wrapped in the exception text the refresh call throws.
		Assert.IsTrue(IsPermanent("Gotrue.Exceptions.GotrueException: {\"code\":403,\"error_code\":\"session_expired\",\"msg\":\"Session expired\"}"), "session_expired");
		Assert.IsTrue(IsPermanent("Session expired"), "bare message");
		Assert.IsTrue(IsPermanent("Invalid Refresh Token: Already Used (refresh_token_already_used)"), "rotated token already spent");
		Assert.IsTrue(IsPermanent("Invalid Refresh Token: Refresh Token Not Found"), "refresh token gone");
		Assert.IsTrue(IsPermanent("{\"error_code\":\"session_not_found\"}"), "session row gone");
		Assert.IsTrue(IsPermanent("invalid_grant"), "oauth style");
	}

	[Test]
	public void RecoverableFailuresAreNotPermanent()
	{
		// These must stay transient: forcing a sign-in screen on a flaky connection would throw the
		// player out of a session that is still good.
		Assert.IsFalse(IsPermanent("System.Net.Http.HttpRequestException: Connection refused"), "connection");
		Assert.IsFalse(IsPermanent("The operation was canceled."), "cancellation");
		Assert.IsFalse(IsPermanent("Name or service not known"), "dns");
		Assert.IsFalse(IsPermanent("HTTP 503 Service Temporarily Unavailable"), "gateway");
		Assert.IsFalse(IsPermanent(""), "no message at all");
		Assert.IsFalse(IsPermanent(null), "null");
	}
}
