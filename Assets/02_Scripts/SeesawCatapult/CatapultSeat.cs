using UnityEngine;

namespace SeesawCatapult
{
    public class CatapultSeat : MonoBehaviour
    {
        private bool _isSeatFull;

        public bool IsSeatFull()
        {
            return _isSeatFull;
        }

        public void SetIsSeatFull(bool isFull)
        {
            _isSeatFull = isFull;
        }
    }
}
