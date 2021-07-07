using Assets.Scripts.events;
using Assets.Scripts.events.handlers;
using ev;
using UnityEngine;

namespace Assets.Scripts {
    public class CameraController : MonoBehaviour {

        [SerializeField]
        private GameObject cameraObject;
        [SerializeField]
        private MouseMoveEvent mouseMoveEvent;
        public float speed;


        private void Start() {
            var mouseToken = new Token();
            mouseMoveEvent.handler.AddListener(mouseToken, RotateCamera);

        }

        private void RotateCamera(MouseEvent mouseEvent) {
            cameraObject.transform.Rotate(0, mouseEvent.axisX * Time.deltaTime * speed, 0);
        }
    }

}

