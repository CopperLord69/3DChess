using events.handlers;
using chess;
using UnityEngine;
using events;

public class FigurePicker : MonoBehaviour {
    private new Camera camera;

    [SerializeField]
    private LayerMask figureMask;
    [SerializeField]
    private LayerMask fieldMask;

    public FigurePickEvent pickEvent;
    public FigureMoveEvent moveEvent;

    private void Awake() {
        camera = Camera.main;
        pickEvent = new FigurePickEvent();
        moveEvent = new FigureMoveEvent();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var endPos = Input.mousePosition;
            var start = camera.transform.position;
            endPos.z = camera.farClipPlane;
            endPos = camera.ScreenToWorldPoint(endPos);
            if (Physics.Linecast(start, endPos, out RaycastHit hit, figureMask)) {
                pickEvent.handler.Push(new FigPickEvent { figure = hit.collider.gameObject });
            } else {
                if (Physics.Linecast(start, endPos, out RaycastHit hitField, fieldMask)) {
                    Vector3 movePosition = hitField.point;
                    movePosition.y = 1;
                    movePosition.x = Mathf.RoundToInt(movePosition.x);
                    movePosition.z = Mathf.RoundToInt(movePosition.z);
                    moveEvent.handler.Push(new FigMoveEvent { position = movePosition });
                }
            }
        }
    }
}
