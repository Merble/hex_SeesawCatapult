using UnityEngine;

namespace AwesomeGame
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
