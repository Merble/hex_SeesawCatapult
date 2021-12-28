using System.Collections;
using AwesomeGame.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AwesomeGame
{
    public class Human : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider _CapsuleCollider;
        [SerializeField] private Rigidbody _Rigidbody;
        [SerializeField] private Human _Prefab;
        [SerializeField] private Transform _TopPoint;
        [SerializeField] private Animator _Animator;
        [SerializeField] private ParticleSystem _DestroyEffect;
        [Space]
        [SerializeField] private HumanType _Type;
        [SerializeField] private Team _Team;
        [Space]
        [SerializeField] private float _Mass;
        [SerializeField] private float _MaxScale;
        [SerializeField] private float _ScaleChangeDuration;
        [Space]
        [ShowInInspector, ReadOnly] private HumanState _state = HumanState.Idle;

        private Vector3 _moveSpot;

        private float _minMoveSpeed;
        private float _maxMoveSpeed;
        private float _maxX;
        private float _minX;
        private float _maxZ;
        private float _minZ;
        
        private static readonly int Running = Animator.StringToHash("IsRunning");
        private static readonly int Sit = Animator.StringToHash("Sit");
        private static readonly int Fall = Animator.StringToHash("Fall");
        private int? _randomMoveTweenId;

        private bool IsRunning => (_state == HumanState.RandomMove) || 
                                  (_state == HumanState.IsMovingToCatapult) ||
                                  (_state == HumanState.IsMovingToSeesaw);

        public Human Prefab => _Prefab;
        public HumanType Type => _Type;
        public float Mass => _Mass;

        public HumanState State => _state;

        public Team Team
        {
            get => _Team;
            set => _Team = value;
        }

        private void Awake()
        {
            _Animator.SetBool(Running, IsRunning);
            SetIsPhysics(false);
        }

        public void MoveToCatapult(Catapult catapult)
        {
            _state = HumanState.IsMovingToCatapult;
        
            if(_randomMoveTweenId != null) LeanTween.cancel(_randomMoveTweenId.Value);
        
            var startPos = transform.position;
            var catapultPos = catapult.GetSeatPosition();
            var moveDuration = Vector3.Distance(startPos, catapultPos) / _maxMoveSpeed;
            
            // Move to seat position
            LeanTween.value(gameObject, 0, 1, moveDuration)
                .setOnUpdate(val =>
                {
                    var pos = Vector3.Lerp(startPos, catapult.GetSeatPosition(), val);
                    transform.position = pos;
                    transform.LookAt(pos);
                })
                .setOnComplete(() =>
                {
                    _Animator.SetTrigger(Sit);
                    transform.LookAt(Vector3.back);
                    catapult.DidHumanCome(this);
                });
        }
        
        public void MoveToSeesaw(SeesawSeat seat)
        {
            _state = HumanState.IsMovingToSeesaw;
            
            seat.HumanAddedToSeat();
        
            if(_randomMoveTweenId != null) LeanTween.cancel(_randomMoveTweenId.Value);
            
            var startPos = transform.position;
            var seatPos = seat.GetSeatPosition();
            seat.SetSeatPosition(_TopPoint.localPosition.y);
            var moveDuration = Vector3.Distance(startPos, seatPos) / _maxMoveSpeed;
            
            LeanTween.value(gameObject, 0, 1, moveDuration)
                .setOnUpdate(val =>
                {
                    var pos = Vector3.Lerp(startPos, seatPos, val);
                    transform.position = pos;
                    transform.LookAt(pos);
                })
                .setOnComplete(() =>
                {
                    _state = HumanState.OnSeesaw;
                
                    _Animator.SetTrigger(Sit);

                    var branch = seat.GetComponentInParent<SeesawBranch>();
                    branch.AddHuman(this);
            
                    transform.SetParent(branch.GetComponentInParent<Seesaw>().transform);
                });
        }
    
        public IEnumerator MoveRandomLocation()
        {
            while (_state == HumanState.RandomMove)
            {
                var position = transform.position;
                var posX = Random.Range(_minX, _maxX);
                var posZ = Random.Range(_minZ, _maxZ);
                _moveSpot = new Vector3(posX, position.y, posZ);

                var moveSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
                var moveDuration = Vector3.Distance(position, _moveSpot) / moveSpeed;

                transform.LookAt(_moveSpot);
            
                _randomMoveTweenId = LeanTween.move(gameObject, _moveSpot, moveDuration).id;

                yield return new WaitForSeconds(moveDuration);
            }
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
            
            SetIsPhysics(false);
            
            _Animator.SetTrigger(Fall);
            
            if (board.Team != Team)
            {
                _state = HumanState.RandomMove;
                StartCoroutine(MoveRandomLocation());
            }
            else
                _state = HumanState.OnOtherSide;
        }

        public void DestroySelf()
        {
            LeanTween.scale(gameObject, Vector3.one * _MaxScale, _ScaleChangeDuration).setOnComplete(() =>
            {
                _DestroyEffect.Play();
                Destroy(gameObject);
            });
        }

        public void Throw(Vector3 throwForce)
        {
            SetIsPhysics(true);
            _Rigidbody.AddForce(throwForce, ForceMode.VelocityChange);
            _state = HumanState.IsFlying;
        }
        
        private void SetIsPhysics(bool isPhysics)
        {
            _Rigidbody.isKinematic = !isPhysics;
            _CapsuleCollider.enabled = isPhysics;
        }
    }
}
