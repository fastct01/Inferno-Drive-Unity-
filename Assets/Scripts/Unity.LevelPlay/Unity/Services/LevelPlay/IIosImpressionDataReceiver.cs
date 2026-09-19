namespace Unity.Services.LevelPlay
{
	internal interface IIosImpressionDataReceiver
	{
		void InvokeImpressionDataReadyEvent(string impressionDataJson);
	}
}
