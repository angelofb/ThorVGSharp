namespace ThorVGSharp.Tests;

internal static class TestApiAssert
{
    public static void AllowsTvgException(Action action)
    {
        var ex = Record.Exception(action);
        if (ex is not null)
            Assert.IsAssignableFrom<TvgException>(ex);
    }

    public static T AllowsTvgException<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            Assert.IsAssignableFrom<TvgException>(ex);
            return default!;
        }
    }
}
