using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP .Client
{
    public static class ClientScenes
    {
        [System.Serializable]
        public enum SceneList 
        {
            Launcher = 0,
            LoginScene =1,
            CharacterSelectScene =2,
            CharacterCreateScene =3,
            VRCalibration = 4,
            Gameplay = 5,
            Arena = 6
        }
    }
}