using HexGames;
using UnityEngine;

namespace SeesawCatapult.ThisGame.Main
{
    public class LevelHelper: LevelHelperBase
    {

        [SerializeField] private Player _Player;
        [SerializeField] private EnemyAI _EnemyAI;
        [SerializeField] private SeesawManager _SeesawManager;

        public SeesawManager SeesawManager => _SeesawManager;

        public Player Player => _Player;

        public EnemyAI EnemyAI => _EnemyAI;

        private void OnValidate()
        {
            if (!_SeesawManager) _SeesawManager = FindObjectOfType<SeesawManager>();
            if (!_Player) _Player = FindObjectOfType<Player>();
            if (!_EnemyAI) _EnemyAI = FindObjectOfType<EnemyAI>();
        }
    }
}