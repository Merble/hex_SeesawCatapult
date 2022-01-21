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

        public HumanManager RivalHumanManager { get; set; }
        public HumanManager HumanManager => _HumanManager;
        public PowerUpManager PowerUpManager => _PowerUpManager;
        
        private void Awake()
        {
            _HumanManager.Catapult = _Catapult;
        
            _Catapult.HumanDidComeToCatapult += OnHumanArriveCatapult;
            _Catapult.DidThrowHumans += humans => { _PowerUpManager.HumanGroupList.Add(humans); };
            _Catapult.DidSeatsFilledUp += () => { _HumanManager.SetIsCatapultAvailable(false); };
            _Catapult.DidSeatsGotEmpty += () => { _HumanManager.SetIsCatapultAvailable(true); };
        
            _PowerUpManager.DidInstantiateHumans += humans => _HumanManager.AddHumans(humans);
        }
    
        private void OnHumanArriveCatapult(Human human)
        {
            if( (_Catapult.HumansOnCatapult.Count >= Game.Config._EnemyHumanNumberBeforeThrow) || (_HumanManager._HumansToCreate <= 0) )
                _Catapult.ThrowHumansByPosition(FindProperPointForThrow(), Game.Config._EnemyHumanThrowWaitDuration);
        }

        private Vector3 FindProperPointForThrow()
        {
            Vector3 HumanThrowPointRadius(Vector3 pos)
            {
                var humanThrowPointRadius = Random.insideUnitCircle * Game.Config._EnemyHumanThrowPointRadiusMultiplier;

                return new Vector3(pos.x + humanThrowPointRadius.x, 0, pos.z + humanThrowPointRadius.y);
            }
        
            var losingSeesaws = _SeesawManager.GetLosingSeesawsForEnemy();
            if (losingSeesaws.Any())
            {
                var seat = losingSeesaws[Random.Range(0, losingSeesaws.Count)].GetSeesawSeatForEnemyAI();
                if (seat != null)
                {
                    var pos =  seat.transform.position;
                    return HumanThrowPointRadius(pos);
                }
            }
            
            if (_PowerUpManager.PowerUps.Any())
            {
                var pos = _PowerUpManager.PowerUps[Random.Range(0, _PowerUpManager.PowerUps.Count)].transform.position;
                return HumanThrowPointRadius(pos);
            }

            var balancedSeesaws = _SeesawManager.GetBalancedSeesaws();
            if (balancedSeesaws.Any())
            {
                var seat = balancedSeesaws[Random.Range(0, balancedSeesaws.Count)].GetSeesawSeatForEnemyAI();
                if (seat != null)
                {
                    var pos =  seat.transform.position;
                    return HumanThrowPointRadius(pos);
                }
            }

            return RivalHumanManager.GetRandomPointOnBoard();
        }
    }
}
