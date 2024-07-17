using System.ComponentModel.Design;
using UnityEngine;

[CreateAssetMenu(menuName = "Match-3/item")]
public sealed class item : ScriptableObject
{
    public int value;

    public Sprite sprite; 
}
