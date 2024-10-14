/// <summary>
/// Class for easy debug of multiple objects
/// </summary>
public static class HexDebug
{
    public static void Log(params object[] objs) =>
        UnityEngine.Debug.Log(string.Join(" ", objs));

    public static void LogWarning(params object[] objs) =>
        UnityEngine.Debug.LogWarning(string.Join(" ", objs));

    public static void LogError(params object[] objs) =>
        UnityEngine.Debug.LogError(string.Join(" ", objs));
}