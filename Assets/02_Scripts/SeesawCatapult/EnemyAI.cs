using System.Linq;
using AwesomeGame._02_Scripts.SeesawCatapult.Enums;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private HumanManager _HumanManager;
        [SerializeField] private Catapult _Catapult;
        [SerializeField] private PowerUpManager _PowerUpManager;
        [SerializeField] private SeesawManager _SeesawManager;
        [Space] 
        [SerializeField] private float _HumanThrowPointRadiusMultiplier;
        [SerializeField] private int _HumanNumberToThrow;
        [SerializeField] private float _HumanThrowWaitDuration;
        
        public HumanManager RivalHumanManager { get; set; }
        
        private void Awake()
        {
            _HumanManager.Catapult = _Catapult;
        
            _Catapult.HumanDidComeToCatapult += OnHumanArriveCatapult;
            _Catapult.DidThrowHumans += humans =>
            {
                _PowerUpManager.HumanGroupList.Add(humans);
                // _HumanManager.HumansOnOtherSideWaitList.AddRange(humans.ToList());
            };
        
            // _PowerUpManager.DidInstantiateHumans += humans => { _HumanManager.HumansOnOtherSideWaitList.AddRange(humans); };
            _PowerUpManager.DidInstantiateHumans += humans => _HumanManager.AddHumans(humans);
        }
    
        private void OnHumanArriveCatapult(Human human)
        {
            // if (human == null) return;
        
            // human.SetState(HumanState.OnCatapult);
            // human.MakeColliderSmaller();

            // _Catapult.AddHuman(human);
            
            if( (_Catapult.HumansOnCatapult.Count >= _HumanNumberToThrow) || (_HumanManager.HumansToCreate <= 0) )
                _Catapult.ThrowHumansByPosition(FindProperPointForThrow(), _HumanThrowWaitDuration); 
        
            // _HumanManager.HumansOnRandomMove.Remove(human);
            // _HumanManager.MoveHumansToCatapult();
        }

        private Vector3 FindProperPointForThrow()
        {
            Vector3 HumanThrowPointRadius(Vector3 pos)
            {
                var humanThrowPointRadius = Random.insideUnitCircle * _HumanThrowPointRadiusMultiplier;

                return new Vector3(pos.x + humanThrowPointRadius.x, 0, pos.z + humanThrowPointRadius.y);
            }
        
            var losingSeesaws = _SeesawManager.GetLosingSeesawsForEnemy();
            if (losingSeesaws.Any())
            {
                var pos =  losingSeesaws[Random.Range(0, losingSeesaws.Count)].transform.position;
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
                var pos =  balancedSeesaws[Random.Range(0, balancedSeesaws.Count)].transform.position;
                return HumanThrowPointRadius(pos);
            }

            return RivalHumanManager.GetRandomPointOnBoard();
        }
    }
}
