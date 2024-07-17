using UnityEngine;

public static class itemDatabase
{
  public static item[] items { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]private static void Initialize() => items = Resources.LoadAll<item>("items/");


}
