using UnityEngine;

/// <summary>
/// Used as data storage for Accessory data. This script is added to any child object
/// of the Character Prefab (see in Bundles/Characters for sample characters and their accessories).
/// </summary>
public class Skin : MonoBehaviour
{
    public string characterName;
    public int characterStar;
    public int characterCoin;

    public string skinName;
    public string displayName;
    public int skinStar;
    public int skinCoin;

    [Header("Korean Language")]
    public string korDisplayName;

    public Animator animator;

    [Header("Sound")]
    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    //public Sprite accessoryIcon;
}
