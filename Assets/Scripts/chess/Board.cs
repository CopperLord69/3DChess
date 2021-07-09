using ev;
using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class Board : MonoBehaviour {

        [SerializeField]
        private GameState gameState;

        [SerializeField]
        private FigurePicker picker;

        [SerializeField]
        private List<FieldSet> fieldsSets;

        [SerializeField]
        private List<FigureSet> figuresSets;
        private List<ChessFigure> figures = new List<ChessFigure>();

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

        private void Start() {
            int j = 0;
            int i = 0;
            var size = fieldsSets.Count;
            string[][] boardPositions = new string[size][];
            boardPositions[j] = new string[size];
            foreach (var fieldSet in fieldsSets) {
                boardPositions[i] = new string[size];
                foreach (var field in fieldSet.fields) {
                    boardPositions[i][j] = field.name;
                    j++;
                }
                j = 0;
                i++;
            }

            foreach (var figuresSet in figuresSets) {
                figures.AddRange(figuresSet.figures);
            }

            var figureMaterials = new Dictionary<FigureColor, Material>();
            for (int k = 0; k < colors.Count; k++) {
                figureMaterials.Add(colors[k], materials[k]);
            }

            figuresController = new FiguresController(figures, boardPositions);
            Token t = new Token();
            picker.pickEvent.handler.Register(t, SelectFiugre);
        }

        private void SelectFiugre(FigPickEvent e) {
            var figure = e.figure;
            if (figure.color != gameState.currentPlayer) {
                return;
            }
            if (figure != selectedFigure && selectedFigure != null) {
                DeselectFigure(selectedFigure.gameObject);
            }
            var figureObject = figure.gameObject;
            var endPosition = new Vector3(
                figureObject.transform.position.x,
                pickHeight,
                figureObject.transform.position.z);
            selectedFigure = e.figure;
            StartCoroutine(LerpObjectPosition(figureObject, endPosition));
            figuresController.CalculateFigureMoveDirections(selectedFigure);
            ColorizeFields(positionMaterial);
        }

        private void ColorizeFields(Material mat) {
            bool ok;
            foreach (var direction in selectedFigure.moveDirections) {
                foreach (var position in direction) {
                    ok = true;
                    for (int i = 0; i < fieldsSets.Count && ok; i++) {
                        for (int j = 0; j < fieldsSets[i].fields.Count && ok; j++) {
                            if (fieldsSets[i].fields[j].name == position) {
                                fieldsSets[i].fields[j].GetComponent<MeshRenderer>().material
                                    = mat;
                                ok = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DeselectFigure(GameObject figureObject) {
            var endPosition = new Vector3(
                figureObject.transform.position.x,
                1,
                figureObject.transform.position.z);
            StartCoroutine(LerpObjectPosition(figureObject.gameObject, endPosition));
        }

        private IEnumerator LerpObjectPosition(GameObject objectToMove, Vector3 end) {
            var objectPosition = objectToMove.transform.position;
            while (objectPosition != end) {
                objectToMove.transform.position = objectPosition;
                objectPosition = Vector3.Lerp(objectPosition, end, lerpSpeed);
                yield return null;
            }
            var collider = objectToMove.GetComponent<Collider>();
            collider.enabled = !collider.enabled;
        }

        private void MoveFigure(FigMoveEvent e) {
            StartCoroutine(LerpObjectPosition(selectedFigure.gameObject, e.position));
        }
    }
}
