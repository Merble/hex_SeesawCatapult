using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private HumanManager _HumanManager;
    [SerializeField] private Catapult _Catapult;
    [SerializeField] private PowerUpManager _PowerUpManager;
    [Space] 
    [SerializeField] private int _HumanNumberToThrow;
    
    private void Awake()
    {
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
        //if(_Catapult.HumansOnCatapult.Count >= _HumanNumberToThrow)
            //_Catapult.ThrowHumansByPosition(FindProperPointForThrow()); 
        
        _HumanManager.HumansOnRandomMove.Remove(human);
        _HumanManager.MoveHumansToCatapult();
    }

    private Vector3 FindProperPointForThrow()
    {
        return Random.insideUnitCircle; // TODO : After doing an algorithm for this, change Random.insideUnitCircle 'bull something' :)
    }
}
