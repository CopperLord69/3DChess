using ev;
using events;
using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class Board : MonoBehaviour {

        [SerializeField]
        private GameState gameState;

        [SerializeField]
        private FigurePicker picker;

        [SerializeField]
        private List<FigureSet> figuresSets;
        private List<ChessFigure> figures = new List<ChessFigure>();

        [SerializeField]
        private List<FieldSet> fieldSets;


        [SerializeField]
        private List<FigureColor> colors;
        [SerializeField]
        private List<Material> materials;
        [SerializeField]
        private Material positionMaterial;
        [SerializeField]
        private Material dangerMaterial;

        [SerializeField]
        private float lerpSpeed;
        [SerializeField]
        private float pickHeight;

        private FiguresController figuresController;

        private ChessFigure selectedFigure;
        private List<List<Vector2Int>> selectedFigureMoveDirections = new List<List<Vector2Int>>();
        GameObject[][] boardObjects = new GameObject[8][];

        private void Start() {
            var size = 8;
            boardObjects[0] = new GameObject[size];
            for (int i = 0; i < size; i++) {
                boardObjects[i] = new GameObject[size];
                for (int j = 0; j < size; j++) {
                    boardObjects[i][j] = fieldSets[i].fields[j];
                }
            }

            foreach (var figuresSet in figuresSets) {
                figures.AddRange(figuresSet.figures);
            }

            var figureMaterials = new Dictionary<FigureColor, Material>();
            for (int k = 0; k < colors.Count; k++) {
                figureMaterials.Add(colors[k], materials[k]);
            }

            figuresController = new FiguresController(figures, boardObjects.GetLength(0));
            Token t = new Token();
            Token t2 = new Token();
            picker.pickEvent.handler.Register(t, SelectFiugre);
            picker.moveEvent.handler.Register(t2, MakeFigureTurn);
        }

        private void MakeFigureTurn(FigMoveEvent e) {
            var position = new Vector2Int((int)e.position.x, (int)e.position.z);
            selectedFigure.position = position;
            foreach (var direction in selectedFigureMoveDirections) {
                if (direction.Contains(position)) {
                    MoveFigure(e);
                    if (gameState.currentPlayer == FigureColor.Black) {
                        gameState.currentPlayer = FigureColor.White;
                    } else {
                        gameState.currentPlayer = FigureColor.Black;
                    }
                    return;
                }
            }
        }

        private void SelectFiugre(FigPickEvent e) {
            selectedFigureMoveDirections.Clear();
            var figure = e.figure;
            if (figure.color != gameState.currentPlayer) {
                return;
            }
            if (figure != selectedFigure && selectedFigure != null) {
                DeselectFigure(selectedFigure.gameObject);
            }
            var figureObject = figure.gameObject;
            var endPosition = new Vector3(
                figureObject.transform.localPosition.x,
                pickHeight,
                figureObject.transform.localPosition.z);
            selectedFigure = e.figure;
            MoveFigure(endPosition);
            selectedFigureMoveDirections =
                figuresController.CalculateFigureMoveDirections(selectedFigure);

            ColorizeFields(positionMaterial);
        }

        private void ColorizeFields(Material mat) {
            foreach (var direction in selectedFigureMoveDirections) {
                foreach (var position in direction) {
                    var pos = new Vector3(position.x, 0, position.y);
                    foreach (var fieldSet in fieldSets) {

                        var field = fieldSet.fields.Find(field => field.transform.localPosition == pos);
                        if (field != null) {
                            field.GetComponent<MeshRenderer>().material = mat;
                        }
                    }
                }
            }
            foreach (var direction in selectedFigureMoveDirections) {
                foreach (var position in direction) {

                }
            }
        }

        private void DeselectFigure(GameObject figureObject) {
            var endPosition = new Vector3(
                figureObject.transform.localPosition.x,
                1,
                figureObject.transform.localPosition.z);
            MoveFigure(endPosition);
        }


        private void MoveFigure(FigMoveEvent e) {
            MoveFigure(e.position);
        }

        private void MoveFigure(Vector3 endPosition) {
            if (selectedFigure.gameObject.TryGetComponent(out FigureMover mover)) {
                mover.endPosition = endPosition;
            } else {
                var moverComponent = selectedFigure.gameObject.AddComponent<FigureMover>();
                moverComponent.endPosition = endPosition ;
            }
        }

    }
}
