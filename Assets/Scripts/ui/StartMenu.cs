using chess;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ui {
    public class StartMenu : MonoBehaviour {
        public void LoadGame() {
            Board.loadGameOnStart = true;
            StartNewGame();
        }

        public void ExitGame() {
            Application.Quit();
        }

        public void StartNewGame() {
            SceneManager.LoadScene(1);
        }
    }

}

