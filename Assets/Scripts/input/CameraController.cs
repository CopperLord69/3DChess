using UnityEngine;

namespace input {
    public class CameraController : MonoBehaviour {

        [SerializeField]
        private GameObject cameraObject;
        public float speed;

        private void Update() {
            var mouseX = Input.GetAxis("Mouse X");
            if (Input.GetKey(KeyCode.LeftControl) && mouseX != 0) {
                RotateCamera(mouseX);
            }
        }

        private void RotateCamera(float delta) {
            cameraObject.transform.Rotate(0, delta * Time.deltaTime * speed, 0);
        }
    }

}

