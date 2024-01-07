using DotNetInterview.API.Providers;

namespace DotNetInterview.Tests;

public class MockDateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get; }
    public MockDateTimeProvider(DateTime dateTime)
    {
        Now = dateTime;
    }
}