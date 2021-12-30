using SeesawCatapult.Enums;
using UnityEngine;

namespace SeesawCatapult
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Team _Team;

        public Team Team => _Team;
    }
}
