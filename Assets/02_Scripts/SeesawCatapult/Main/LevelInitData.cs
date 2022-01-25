using System;
using HexGames;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SeesawCatapult.Main
{
    [Serializable]
    public class LevelInitData: LevelInitDataBase
    {
        [SerializeField, Required] private GameObject _Test;

    }
}