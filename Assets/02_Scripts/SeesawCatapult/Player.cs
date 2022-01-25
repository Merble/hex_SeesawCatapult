using UnityEngine;

namespace SeesawCatapult
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private HumanManager _HumanManager;
        [SerializeField] private PowerUpManager _PowerUpManager;
        [SerializeField] private Catapult _Catapult;
        [SerializeField] private Trajectory _Trajectory;
        

        public HumanManager HumanManager => _HumanManager;
        
        private  VariableJoystick _joystick;
    
        public void Init(VariableJoystick joystick)
        {
            _joystick = joystick;
            _HumanManager.Catapult = _Catapult;
     
            _joystick.DragDidStart += OnJoystickOnDragDidStart;
            _joystick.DidDrag += OnDrag;
            _joystick.DragDidEnd += OnJoystickOnDragDidEnd;

            _Catapult.DidThrowHumans += humans => { _PowerUpManager.HumanGroupList.Add(humans); };
            _Catapult.DidSeatsFilledUp += () => { _HumanManager.SetIsCatapultAvailable(false) ;};
            _Catapult.DidSeatsGotEmpty += () => { _HumanManager.SetIsCatapultAvailable(true); };

            _PowerUpManager.DidInstantiateHumans += humans => _HumanManager.AddHumans(humans);
        }

        private void OnJoystickOnDragDidEnd(Vector2 direction)
        {
            _Catapult.ThrowHumansByDirection(-direction);
            _Trajectory.HideTrajectory();
        }

        private void OnJoystickOnDragDidStart()
        {
            _Trajectory.ShowTrajectory();
        }

        private void OnDrag(Vector2 direction)
        {
            _Trajectory.UpdateDots(-direction);
            //var finishPos = _Catapult.FindTrajectoryFinishPosition(-direction);
            //_Indicator.transform.position = new Vector3(finishPos.x, _Indicator.transform.position.y, finishPos.z);
        }

        private void OnDestroy()
        {
            _joystick.DragDidStart -= OnJoystickOnDragDidStart;
            _joystick.DidDrag -= OnDrag;
            _joystick.DragDidEnd -= OnJoystickOnDragDidEnd;
        }
    }
}
