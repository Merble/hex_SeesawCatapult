using System.Linq;
using AwesomeGame._02_Scripts.SeesawCatapult.Enums;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private HumanManager _HumanManager;
        [SerializeField] private  VariableJoystick _Joystick;
        [SerializeField] private Catapult _Catapult;
        [SerializeField] private GameObject _Indicator;
        [SerializeField] private PowerUpManager _PowerUpManager;

        public HumanManager HumanManager => _HumanManager;
    
        private void Awake()
        {
            _HumanManager.Catapult = _Catapult;
     
            _Joystick.DragDidStart += () => { _Indicator.SetActive(true); };
            _Joystick.DidDrag += OnDrag;
            _Joystick.DragDidEnd += direction =>
            {
                _Catapult.ThrowHumansByDirection(-direction);
                _Indicator.SetActive(false);
            };

            _Catapult.HumanDidComeToCatapult += OnHumanArriveCatapult;
            _Catapult.DidThrowHumans += humans =>
            {
                _PowerUpManager.HumanGroupList.Add(humans);
                _HumanManager.HumansOnOtherSideWaitList.AddRange(humans.ToList());
            };

            _PowerUpManager.DidInstantiateHumans += humans => { _HumanManager.HumansOnOtherSideWaitList.AddRange(humans); };
        }
    
        private void OnHumanArriveCatapult(Human human)
        {
            if (human == null) return;

            human.SetState(HumanState.OnCatapult);
            human.MakeColliderSmaller();

            _Catapult.AddHuman(human);

            _HumanManager.HumansOnRandomMove.Remove(human);
            _HumanManager.MoveHumansToCatapult();
        }

        private void OnDrag(Vector2 direction)
        { 
            var finishPos = _Catapult.FindTrajectoryFinishPosition(-direction);
            _Indicator.transform.position = new Vector3(finishPos.x, _Indicator.transform.position.y, finishPos.z);
        }
    }
}
