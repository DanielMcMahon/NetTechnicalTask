namespace DotNetInterview.API.Providers;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}