using UnityEngine;

namespace ui.controllers {
    public class MenuController : MonoBehaviour {

        [SerializeField]
        public GameObject menu;


        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ToggleMenu();
            }
        }

        public void ToggleMenu() {

            menu.SetActive(!menu.activeSelf);
            if (menu.activeSelf) {
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;

            }
        }

    }

}

