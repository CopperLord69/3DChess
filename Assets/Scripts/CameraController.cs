using Assets.Scripts.events;
using Assets.Scripts.events.handlers;
using ev;
using UnityEngine;

namespace Assets.Scripts {
    public class CameraController : MonoBehaviour {

        [SerializeField]
        private GameObject cameraObject;

        private MouseMoveEvent mouseMoveEvent = new MouseMoveEvent();
        private bool menuNotOpened = true;

        private void Awake() {
            var mouseToken = new Token();
            mouseMoveEvent.handler.AddListener(mouseToken, RotateCamera);
            
        }

        private void Update() {
            if (menuNotOpened)
            {
                var mouseDeltaX = Input.GetAxis("Mouse X");
                if (mouseDeltaX != 0)
                {
                    mouseMoveEvent.handler.Invoke(new MouseEvent { axisX = mouseDeltaX, axisY = 0 });
                }
            }
        }

        private void RotateCamera(MouseEvent mouseEvent) {
            cameraObject.transform.Rotate(0, mouseEvent.axisX, 0);
        }
    }

}

