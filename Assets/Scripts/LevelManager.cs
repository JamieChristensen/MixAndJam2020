using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class LevelManager : SerializedScriptableObject
{
    [SerializeField]
    private Level[] AllLevels;

    public int CurrentLevel;

    public Level GetCurrentLevel() => AllLevels[CurrentLevel];


}
