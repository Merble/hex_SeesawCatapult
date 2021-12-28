using AwesomeGame.Enums;
using UnityEngine;

namespace AwesomeGame
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Team _Team;

        public Team Team => _Team;
    }
}
