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
            Debug.DrawLine(start, endPos, Color.yellow, 1);
            if (Physics.Linecast(start, endPos, out RaycastHit hit, figureMask)) {
                var figureComponent = hit.collider.GetComponent<ChessFigure>();
                var position = hit.collider.gameObject.transform.localPosition;
                var figPosition = new Position(
                    Mathf.RoundToInt(position.x),
                    Mathf.RoundToInt(position.z
                    ));
                figureComponent.position = figPosition;
                pickEvent.handler.Push(new FigPickEvent { figure = figureComponent });
            } else {
                if (Physics.Linecast(start, endPos, out RaycastHit hitField, fieldMask)) {
                    Vector3 movePosition = hitField.point;
                    Debug.DrawRay(movePosition, Vector3.up, Color.yellow, 4);
                    movePosition.y = 1;
                    movePosition.x = Mathf.RoundToInt(movePosition.x);
                    movePosition.z = Mathf.RoundToInt(movePosition.z);
                    moveEvent.handler.Push(new FigMoveEvent { position = movePosition });
                }
            }
        }
    }
}
