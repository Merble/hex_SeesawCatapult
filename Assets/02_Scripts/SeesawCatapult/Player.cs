using UnityEngine;

namespace SeesawCatapult
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private HumanManager _HumanManager;
        [SerializeField] private PowerUpManager _PowerUpManager;
        [SerializeField] private Catapult _Catapult;
        [SerializeField] private GameObject _Indicator;
        [SerializeField] private Trajectory _Trajectory;
        

        public HumanManager HumanManager => _HumanManager;
        
        private  VariableJoystick _joystick;
    
        public void Init(VariableJoystick joystick)
        {
            _joystick = joystick;
            _HumanManager.Catapult = _Catapult;
     
            _joystick.DragDidStart += () => { _Trajectory.ShowTrajectory(); };
            _joystick.DidDrag += OnDrag;
            _joystick.DragDidEnd += direction =>
            {
                _Catapult.ThrowHumansByDirection(-direction);
                _Trajectory.HideTrajectory();
            };

            _Catapult.DidThrowHumans += humans => { _PowerUpManager.HumanGroupList.Add(humans); };
            _Catapult.DidSeatsFilledUp += () => { _HumanManager.SetIsCatapultAvailable(false) ;};

            _PowerUpManager.DidInstantiateHumans += humans => _HumanManager.AddHumans(humans);
        }
    
        private void OnDrag(Vector2 direction)
        {
            _Trajectory.UpdateDots(-direction);
            //var finishPos = _Catapult.FindTrajectoryFinishPosition(-direction);
            //_Indicator.transform.position = new Vector3(finishPos.x, _Indicator.transform.position.y, finishPos.z);
        }

        private void OnDestroy()
        {
            
        }
    }
}
