using UnityEngine;

public static class Keybinds
{
    public static GameActions Actions;
    
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Initialize()
    {
        Actions = new GameActions();
    }
}
