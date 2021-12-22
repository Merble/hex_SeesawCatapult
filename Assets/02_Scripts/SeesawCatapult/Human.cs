using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Human : MonoBehaviour
{
    
    
    [SerializeField] private CapsuleCollider _CapsuleCollider;
    [SerializeField] private Rigidbody _Rigidbody;
    [SerializeField] private Human _Prefab;
    [Space]
    [SerializeField] private HumanType _Type;
    [SerializeField] private Team _Team;
    [Space]
    [SerializeField] private float _Mass;
    [SerializeField] private float _ColliderRadiusMin;
    [SerializeField] private float _ColliderRadiusMax;
    [SerializeField] private float _MinScale;
    [SerializeField] private float _MaxScale;
    [SerializeField] private float _ScaleChangeDuration;
    [Space]
    [ShowInInspector, ReadOnly] private HumanState _state = HumanState.Idle;

    private int? _randomMoveTweenId;
    private Vector3 _moveSpot;

    private float _minMoveSpeed;
    private float _maxMoveSpeed;
    private float _maxX;
    private float _minX;
    private float _maxZ;
    private float _minZ;

    public Human Prefab => _Prefab;
    public HumanType Type => _Type;
    public Rigidbody Rigidbody => _Rigidbody;
    public float Mass => _Mass;

    public Team Team
    {
        get => _Team;
        set => _Team = value;
    }

    public void MoveToCatapult(Catapult catapult)
    {
        _state = HumanState.IsMovingToCatapult;
        
        if(_randomMoveTweenId != null) LeanTween.cancel(_randomMoveTweenId.Value);
        
        var position = transform.position;
        var catapultPos = catapult.transform.position;
        var newPos = new Vector3(catapultPos.x, position.y, catapultPos.z);
        var moveDuration = Vector3.Distance(position, newPos) / _maxMoveSpeed;

        LeanTween.move(gameObject, newPos, moveDuration).setOnComplete(() =>
        {
            catapult.DidHumanCome(this);
        });
    }
    
    public void MoveToSeesaw(SeesawSeat seat)
    {
        _state = HumanState.IsMovingToSeesaw;
        
        if(_randomMoveTweenId != null) LeanTween.cancel(_randomMoveTweenId.Value);

        var pos = transform.position;
        var newPos = seat.transform.position;
        var moveDuration = Vector3.Distance(pos, newPos) / _maxMoveSpeed;
        
        
        
        LeanTween.move(gameObject, newPos, moveDuration).setOnComplete(() =>
        {
            _state = HumanState.OnSeesaw;
            
            var branch = seat.GetComponentInParent<SeesawBranch>();
            branch.AddHuman(this);
            
            transform.SetParent(branch.GetComponentInParent<Seesaw>().transform);
        });
    }
    
    public IEnumerator MoveRandomLocation()
    {
        if (_state != HumanState.RandomMove) yield break;
        
        var position = transform.position;
        var posX = Random.Range(_minX, _maxX);
        var posZ = Random.Range(_minZ, _maxZ);
        _moveSpot = new Vector3(posX, position.y, posZ);
            
        var moveSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
        var moveDuration = Vector3.Distance(position, _moveSpot) / moveSpeed;

        _randomMoveTweenId = LeanTween.move(gameObject, _moveSpot, moveDuration).id;

        yield return new WaitForSeconds(moveDuration);
        StartCoroutine(MoveRandomLocation());
    }
    
    public void MakeColliderSmaller()
    {
        _CapsuleCollider.radius = _ColliderRadiusMin;
    }
    public void MakeColliderBigger()
    {
        _CapsuleCollider.radius = _ColliderRadiusMax;
    }

    public void SetMinAndMaxValues(float minX, float maxX, float minZ, float maxZ, float minSpeed, float maxSpeed)
    {
        _minX = minX;
        _maxX = maxX;
        
        _minZ = minZ;
        _maxZ = maxZ;
        
        _minMoveSpeed = minSpeed;
        _maxMoveSpeed = maxSpeed;
    }
    
    public HumanState GetState()
    {
        return _state;
    }
    public void SetState(HumanState newState)
    {
        _state = newState;
    }

    private void OnCollisionEnter(Collision other)
    {
        CheckIfGrounded(other.gameObject);
    }

    private void CheckIfGrounded(GameObject other)
    {
        var board = other.GetComponent<Board>();
        
        if (!board) return;
        if (_state != HumanState.IsFlying) return;
        if (board.Team != Team)
        {
            _state = HumanState.OnSameSide;
            return;
        }
            
        _state = HumanState.OnOtherSide;
        MakeColliderBigger();
    }

    public void DestroySelf()
    {
        LeanTween.scale(gameObject, Vector3.one * _MinScale, _ScaleChangeDuration).setOnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
