using Assets.Scripts.events.handlers;
using chess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using events;

public class FigurePicker : MonoBehaviour {
    private new Camera camera;

    [SerializeField]
    private LayerMask figureMask;

    public FigurePickEvent pickEvent;

    private void Awake() {
        camera = Camera.main;
        pickEvent = new FigurePickEvent();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var endPos = Input.mousePosition;
            var start = camera.transform.position;
            endPos.z = camera.farClipPlane;
            endPos = camera.ScreenToWorldPoint(endPos);
            Debug.DrawLine(start, endPos, Color.yellow, 1);
            if (Physics.Linecast(start, endPos, out RaycastHit hit,figureMask)) {
                var figureComponent = hit.collider.GetComponent<ChessFigure>();
                pickEvent.handler.Push(new FigPickEvent { figure = figureComponent });
            }
        }
    }
}
