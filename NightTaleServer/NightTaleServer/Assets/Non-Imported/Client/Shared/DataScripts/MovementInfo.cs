using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Shared
{
    [CreateAssetMenu(menuName = "FYP/Data/MovementData")]
    public class MovementInfo : ScriptableObject
    {
        [SerializeField]
        private float _forwardSpeed = 2f;
        [SerializeField]
        private float _sideSpeed = 1.5f;
        [SerializeField]
        private float _backSpeed = 1.5f;
        [SerializeField]
        private float _sprintMultiplier = 2f;

        public float ForwardSpeed { get => _forwardSpeed;}
        public float SideSpeed { get => _sideSpeed;  }
        public float BackSpeed { get => _backSpeed; }
        public float SprintMultiplier { get => _sprintMultiplier;  }
    }
}