using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.managers {
    public class GameManager : MonoBehaviour {

        private GameState gameState;

        public void QuitGame() {
            Debug.Log("i should be closed, but i'm not work in debug mode");
            Application.Quit();
        }
    }
}


