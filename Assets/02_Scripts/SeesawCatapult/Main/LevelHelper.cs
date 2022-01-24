using HexGames;
using UnityEngine;

namespace SeesawCatapult.ThisGame.Main
{
    public class LevelHelper: LevelHelperBase
    {

        [SerializeField] private Player _Player;
        [SerializeField] private EnemyAI _EnemyAI;
        [SerializeField] private SeesawManager _SeesawManager;
        [SerializeField] private PowerUpCreator _PowerUpCreator;
        [Space] 
        [SerializeField] private int _PowerUpsToCreate;
        [SerializeField] private int _HumansToCreate;

        public SeesawManager SeesawManager => _SeesawManager;
        public Player Player => _Player;
        public EnemyAI EnemyAI => _EnemyAI;

        private void OnValidate()
        {
            if (!_SeesawManager) _SeesawManager = FindObjectOfType<SeesawManager>();
            if (!_Player) _Player = FindObjectOfType<Player>();
            if (!_EnemyAI) _EnemyAI = FindObjectOfType<EnemyAI>();

            _Player.HumanManager._HumansToCreate = _HumansToCreate;
            _EnemyAI.HumanManager._HumansToCreate = _HumansToCreate;
            
            _PowerUpCreator._PowerUpsToCreate = _PowerUpsToCreate;
        }
    }
}