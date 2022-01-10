using System.Collections;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeesawCatapult
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
        [Space]
        [ShowInInspector, ReadOnly] private HumanState _state = HumanState.Idle;

        private Vector3 _moveSpot;

        private float _minMoveSpeed;
        private float _maxMoveSpeed;
        private float _maxX;
        private float _minX;
        private float _maxZ;
        private float _minZ;
        
        private static readonly int RunAnimParam = Animator.StringToHash("Run");
        private static readonly int SitAnimParam = Animator.StringToHash("Sit");
        private static readonly int FallAnimParam = Animator.StringToHash("Fall");
        private int? _randomMoveTweenId;

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
            SetIsPhysics(false);
        }
        
        public IEnumerator MoveRandomLocation()
        {
            _Animator.SetTrigger(RunAnimParam);
            while (_state == HumanState.RandomMove)
            {
                var position = transform.position;
                var posX = Random.Range(_minX, _maxX);
                var posZ = Random.Range(_minZ, _maxZ);
                _moveSpot = new Vector3(posX, 0, posZ);

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

        public void MoveToCatapult(Catapult catapult)
        {
            _state = HumanState.IsMovingToCatapult;
        
            if(_randomMoveTweenId != null) LeanTween.cancel(_randomMoveTweenId.Value);
        
            var startPos = transform.position;
            var catapultPos = catapult.GetSeatPosition();
            var catapultSeatPos = catapult.WhereToSit();
            catapult.SetSeatPosition(_TopPoint.localPosition.y);
            var moveDuration = Vector3.Distance(startPos, catapultPos) / _maxMoveSpeed;
            
            // Move to seat position
            LeanTween.value(gameObject, 0, 1, moveDuration)
                .setOnUpdate(val =>
                {
                    var pos = Vector3.Lerp(startPos, catapultPos, val);
                    transform.position = pos;
                    transform.LookAt(catapultPos);
                })
                .setOnComplete(() =>
                {
                    _Animator.SetTrigger(SitAnimParam);
                    transform.LookAt(Vector3.back);
                    catapult.DidHumanCome(this);
                    transform.position = catapultSeatPos;
                });
        }
        
        public void MoveToSeesaw(SeesawSeat seat)
        {
            var cachedTransform = transform;
            
            seat.HumanAddedToSeat();
        
            if(_randomMoveTweenId != null) LeanTween.cancel(_randomMoveTweenId.Value);
            
            var startPos = transform.position;
            var seatPos = seat.GetSeatPosition();
            seat.SetSeatPosition(_TopPoint.localPosition.y);
            var moveDuration = Vector3.Distance(startPos, seatPos) / _maxMoveSpeed;
            
            var branch = seat._ParentBranch;
            cachedTransform.SetParent(branch._ParentSeesaw.transform);
            branch.AddHuman(this);

            startPos = cachedTransform.localPosition;
            
            cachedTransform.LookAt(seatPos);
            
            // Move to seat position
            LeanTween.value(gameObject, 0, 1, moveDuration)
                .setOnUpdate(val =>
                {
                    var pos = Vector3.Lerp(startPos, seatPos, val);
                    transform.localPosition = pos;
                })
                .setOnComplete(() =>
                {

                    var pos = transform.position;
                    _state = HumanState.OnSeesaw;
                    transform.LookAt(new Vector3(pos.x, pos.y ,0));
                    _Animator.SetTrigger(SitAnimParam);
                });
        }
    
        public void SetState(HumanState newState)
        {
            _state = newState;
        }

        private void OnCollisionEnter(Collision other)
        {
            var board = other.gameObject.GetComponent<Board>();
            if (!board) return;
            if (_state != HumanState.IsFlying) return;
            
            SetIsPhysics(false);
            
            _Animator.SetTrigger(FallAnimParam);
            
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
            LeanTween.scale(gameObject, Vector3.one * Game.Config.HumanMaxScaleRate, Game.Config.HumanScaleChangeDuration).setOnComplete(() =>
            {
                var destroyEffect = Instantiate(_DestroyEffect,transform.position, Quaternion.identity);
                destroyEffect.Play();
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
