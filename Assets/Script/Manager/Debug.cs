public static class DebugOptimum
{
    [System.Diagnostics.Conditional(("UNITY_EDITOR"))]
    public static void Log(object msg) =>
        UnityEngine.Debug.Log(msg);
}