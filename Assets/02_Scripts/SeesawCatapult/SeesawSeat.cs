using Unity.Collections;
using UnityEngine;

namespace AwesomeGame
{
    public class SeesawSeat : MonoBehaviour
    {
        [SerializeField] private int _MaxHumanToSit;
        [SerializeField, ReadOnly] private int _SatHumans;

        private Vector3 _seatPosition;
        public bool IsSeatFull => _SatHumans >= _MaxHumanToSit;

        public SeesawBranch _ParentBranch;
        
        private void Awake()
        {
            _seatPosition = transform.localPosition;
        }

        public void HumanAddedToSeat()
        {
            _SatHumans++;
        }
        
        public void SetSeatPosition(float newY)
        {
            _seatPosition += new Vector3(0, newY, 0);
        }
        
        public Vector3 GetSeatPosition()
        {
            return _seatPosition;
        }
        public void ClearSeat()
        {
            _SatHumans = 0;
            _seatPosition = transform.localPosition;
        }
        
    }
}
