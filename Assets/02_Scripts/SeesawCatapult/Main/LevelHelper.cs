using HexGames;
using UnityEngine;

namespace SeesawCatapult.ThisGame.Main
{
    public class LevelHelper: LevelHelperBase
    {

        [SerializeField] private Player _Player;
        [SerializeField] private EnemyAI _EnemyAI;
        [SerializeField] private SeesawManager _SeesawManager;
        [Space] 
        [SerializeField] private int _PowerUpsToCreate;
        [SerializeField] private int _HumansToCreate;

/*#if UNITY_EDITOR
        [ShowInInspector]
        private int PowerUpsToCreate
        {
            get => FindObjectOfType<PowerUpManager>().PowerUpsToCreate;
            set
            {
                var objs = FindObjectsOfType<PowerUpManager>();
                foreach (var powerUpManager in objs)
                {
                    powerUpManager.PowerUpsToCreate = value;
                }
            }
        }

        [ShowInInspector]
        private int HumansToCreate
        {
            get => FindObjectOfType<HumanManager>().HumansToCreate;
            set
            {
                var objs = FindObjectsOfType<HumanManager>();
                foreach (var humanManager in objs)
                {
                    humanManager.HumansToCreate = value;
                }
            }
        }
#endif
*/
        
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
            
            _Player.PowerUpManager._PowerUpsToCreate = _PowerUpsToCreate;
            _EnemyAI.PowerUpManager._PowerUpsToCreate = _PowerUpsToCreate;
        }
    }
}