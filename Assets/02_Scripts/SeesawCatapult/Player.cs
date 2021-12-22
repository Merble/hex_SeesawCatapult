using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private HumanManager _HumanManager;
    [SerializeField] private  VariableJoystick _Joystick;
    [SerializeField] private Catapult _Catapult;
    [SerializeField] private GameObject _Indicator;
    [SerializeField] private PowerUpManager _PowerUpManager;
    
    private void Awake()
    {
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
            _HumanManager.HumansOnOtherSide.AddRange(humans.ToList());
        };

        _PowerUpManager.DidInstantiateHumans += humans => { _HumanManager.HumansOnOtherSide.AddRange(humans); };
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
