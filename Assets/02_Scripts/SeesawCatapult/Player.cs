using UnityEngine;

namespace SeesawCatapult
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private HumanManager _HumanManager;
        [SerializeField] private Catapult _Catapult;
        [SerializeField] private GameObject _Indicator;
        [SerializeField] private PowerUpManager _PowerUpManager;

        public HumanManager HumanManager => _HumanManager;
        public PowerUpManager PowerUpManager => _PowerUpManager;
        
        private  VariableJoystick _joystick;
    
        public void Init(VariableJoystick joystick)
        {
            _joystick = joystick;
            _HumanManager.Catapult = _Catapult;
     
            _joystick.DragDidStart += () => { _Indicator.SetActive(true); };
            _joystick.DidDrag += OnDrag;
            _joystick.DragDidEnd += direction =>
            {
                _Catapult.ThrowHumansByDirection(-direction);
                _Indicator.SetActive(false);
            };

            _Catapult.DidThrowHumans += humans => { _PowerUpManager.HumanGroupList.Add(humans); };

            _PowerUpManager.DidInstantiateHumans += humans => _HumanManager.AddHumans(humans);
        }
    
        private void OnDrag(Vector2 direction)
        { 
            var finishPos = _Catapult.FindTrajectoryFinishPosition(-direction);
            _Indicator.transform.position = new Vector3(finishPos.x, _Indicator.transform.position.y, finishPos.z);
        }
    }
}
