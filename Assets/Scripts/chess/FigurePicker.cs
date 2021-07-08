using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigurePicker : MonoBehaviour {
    private new Camera camera;

    private void Start() {
        camera = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            print($"{camera.ScreenToWorldPoint(Input.mousePosition)}, {camera.farClipPlane}");
            var pos = Input.mousePosition;
            pos.z = camera.farClipPlane;
            Debug.DrawLine(camera.ScreenToWorldPoint(pos), camera.transform.position, Color.yellow, 1);;
        }
    }
}
