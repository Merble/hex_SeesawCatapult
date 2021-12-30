using System.Linq;
using SeesawCatapult.ThisGame.Main;
using UnityEngine;

namespace SeesawCatapult
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private HumanManager _HumanManager;
        [SerializeField] private Catapult _Catapult;
        [SerializeField] private PowerUpManager _PowerUpManager;
        [SerializeField] private SeesawManager _SeesawManager;
        
        private float HumanThrowPointRadiusMultiplier => Game.Config._EnemyHumanThrowPointRadiusMultiplier;
        private int HumanNumberToThrow => Game.Config._EnemyHumanNumberBeforeThrow;
        private float HumanThrowWaitDuration => Game.Config._EnemyHumanThrowWaitDuration;
        
        public HumanManager RivalHumanManager { get; set; }
        
        private void Awake()
        {
            _HumanManager.Catapult = _Catapult;
        
            _Catapult.HumanDidComeToCatapult += OnHumanArriveCatapult;
            _Catapult.DidThrowHumans += humans => { _PowerUpManager.HumanGroupList.Add(humans); };
        
            _PowerUpManager.DidInstantiateHumans += humans => _HumanManager.AddHumans(humans);
        }
    
        private void OnHumanArriveCatapult(Human human)
        {
            if( (_Catapult.HumansOnCatapult.Count >= HumanNumberToThrow) || (_HumanManager.HumansToCreate <= 0) )
                _Catapult.ThrowHumansByPosition(FindProperPointForThrow(), HumanThrowWaitDuration);
        }

        private Vector3 FindProperPointForThrow()
        {
            Vector3 HumanThrowPointRadius(Vector3 pos)
            {
                var humanThrowPointRadius = Random.insideUnitCircle * HumanThrowPointRadiusMultiplier;

                return new Vector3(pos.x + humanThrowPointRadius.x, 0, pos.z + humanThrowPointRadius.y);
            }
        
            var losingSeesaws = _SeesawManager.GetLosingSeesawsForEnemy();
            if (losingSeesaws.Any())
            {
                var pos =  losingSeesaws[Random.Range(0, losingSeesaws.Count)].GetSeesawSeatForEnemyAI().transform.position;
                return HumanThrowPointRadius(pos);
            }
            
            if (_PowerUpManager.PowerUps.Any())
            {
                var pos = _PowerUpManager.PowerUps[Random.Range(0, _PowerUpManager.PowerUps.Count)].transform.position;
                return HumanThrowPointRadius(pos);
            }

            var balancedSeesaws = _SeesawManager.GetBalancedSeesaws();
            if (balancedSeesaws.Any())
            {
                var pos =  balancedSeesaws[Random.Range(0, balancedSeesaws.Count)].GetSeesawSeatForEnemyAI().transform.position;
                return HumanThrowPointRadius(pos);
            }

            return RivalHumanManager.GetRandomPointOnBoard();
        }
    }
}
