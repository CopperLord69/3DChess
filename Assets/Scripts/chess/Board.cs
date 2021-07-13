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
        private List<Collider> blackColliders;
        private List<Collider> whiteColliders;


        [SerializeField]
        private List<FieldSet> fieldSets;
        private Dictionary<Vector2Int, MeshRenderer> fieldRenderers;


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

        private Dictionary<ChessFigure, bool> availableFigures = new Dictionary<ChessFigure, bool>();
        private void Start() {
            var size = 8;
            boardObjects[0] = new GameObject[size];
            for (int i = 0; i < size; i++) {
                boardObjects[i] = new GameObject[size];
                for (int j = 0; j < size; j++) {
                    boardObjects[i][j] = fieldSets[i].fields[j];
                }
            }

            blackColliders = new List<Collider>();
            whiteColliders = new List<Collider>();
            foreach (var figuresSet in figuresSets) {
                figures.AddRange(figuresSet.figures);
                foreach (var figure in figuresSet.figures) {
                    if (figure.color == FigureColor.Black) {
                        blackColliders.Add(figure.gameObject.GetComponent<Collider>());
                    } else {
                        whiteColliders.Add(figure.gameObject.GetComponent<Collider>());
                    }
                    availableFigures.Add(figure, true);
                }
            }

            var figureMaterials = new Dictionary<FigureColor, Material>();
            for (int k = 0; k < colors.Count; k++) {
                figureMaterials.Add(colors[k], materials[k]);
            }

            figuresController = new FiguresController(figures, boardObjects.GetLength(0));

            fieldRenderers = new Dictionary<Vector2Int, MeshRenderer>();
            foreach (var fieldSet in fieldSets) {
                foreach (var field in fieldSet.fields) {
                    var renderer = field.GetComponent<MeshRenderer>();
                    var position = new Vector2Int(
                        Mathf.RoundToInt(field.transform.localPosition.x),
                        Mathf.RoundToInt(field.transform.localPosition.z)
                        );
                    fieldRenderers.Add(position, renderer);
                }
            }


            Token t = new Token();
            Token t2 = new Token();
            picker.pickEvent.handler.Register(t, SelectFiugre);
            picker.moveEvent.handler.Register(t2, MakeFigureTurn);
        }

        private void MakeFigureTurn(FigMoveEvent e) {
            figures.RemoveAll(fig => fig == null);
            blackColliders.RemoveAll(collider => collider == null);
            whiteColliders.RemoveAll(collider => collider == null);
            var position = new Vector2Int(
                Mathf.RoundToInt(e.position.x),
                Mathf.RoundToInt(e.position.z)
                );
            foreach (var direction in selectedFigureMoveDirections) {
                if (direction.Contains(position)) {
                    selectedFigure.position = position;
                    bool blackCollidersEnabled;
                    bool whiteCollidersEnabled;
                    if (gameState.currentPlayer == FigureColor.Black) {
                        blackCollidersEnabled = false;
                        whiteCollidersEnabled = true;
                        gameState.currentPlayer = FigureColor.White;
                    } else {
                        blackCollidersEnabled = true;
                        whiteCollidersEnabled = false;
                        gameState.currentPlayer = FigureColor.Black;
                    }
                    foreach (var collider in whiteColliders) {
                        collider.enabled = whiteCollidersEnabled;
                    }
                    foreach (var collider in blackColliders) {
                        collider.enabled = blackCollidersEnabled;
                    }
                    ReturnMaterials();
                    var enemies = figures.FindAll(enemy => enemy.color != selectedFigure.color);
                    foreach (var enemy in enemies) {
                        if (enemy.position == position) {
                            figures.Remove(enemy);
                            Destroy(enemy.gameObject);
                            break;
                        }
                    }
                    selectedFigureMoveDirections.Clear();
                    CheckKingDanger();
                    MoveFigure(e);
                    return;
                }
            }
        }

        private void SelectFiugre(FigPickEvent e) {
            var figure = e.figure;
            if (figure.color != gameState.currentPlayer || !availableFigures[figure]) {
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
            selectedFigureMoveDirections.Clear();
            selectedFigureMoveDirections =
                figuresController.CalculateFigureMoveDirections(selectedFigure);
            ColorizeMoveFields();
            ColorizeDangerFields();
        }

        private void ColorizeDangerFields() {

            foreach (var direction in selectedFigureMoveDirections) {
                foreach (var otherFigure in figures) {
                    if (direction.Contains(otherFigure.position)) {
                        if (otherFigure.color != gameState.currentPlayer) {
                            fieldRenderers[otherFigure.position].material = dangerMaterial;
                        }
                    }
                }
            }
        }

        private void ColorizeMoveFields() {
            foreach (var direction in selectedFigureMoveDirections) {
                foreach (var position in direction) {
                    fieldRenderers[position].material = positionMaterial;
                }
            }
        }

        private void DeselectFigure(GameObject figureObject) {
            var endPosition = new Vector3(
                figureObject.transform.localPosition.x,
                1,
                figureObject.transform.localPosition.z);
            MoveFigure(endPosition);
            ReturnMaterials();
        }

        private void MoveFigure(FigMoveEvent e) {
            MoveFigure(e.position);
        }

        private void MoveFigure(Vector3 endPosition) {
            if (selectedFigure.gameObject.TryGetComponent(out FigureMover mover)) {
                mover.endPosition = endPosition;
            } else {
                var moverComponent = selectedFigure.gameObject.AddComponent<FigureMover>();
                moverComponent.endPosition = endPosition;
            }
        }

        private void ReturnMaterials() {
            foreach (var direction in selectedFigureMoveDirections) {
                foreach (var position in direction) {
                    int index = position.x + position.y;
                    if (index % 2 == 0) {
                        index = 0;
                    } else {
                        index = 1;
                    }
                    fieldRenderers[position].material = materials[index];
                }
            }
        }

        private void CheckKingDanger() {
            var kings = figures.FindAll(figure => figure.type == Figure.King);
            foreach (var king in kings) {
                var enemyFigures = figures.FindAll(other => other.color != king.color);
                var allyFigures = figures.FindAll(other => other.color == king.color);
                allyFigures.Remove(king);
                var kingMoves = figuresController.CalculateFigureMoveDirections(king);
                foreach (var enemy in enemyFigures) {
                    var enemyMoveDirecitons = figuresController.CalculateFigureMoveDirections(enemy);
                    foreach (var enemyDirection in enemyMoveDirecitons) {
                        if (enemyDirection.Contains(king.position)) {
                            fieldRenderers[king.position].material = dangerMaterial;
                            foreach (var ally in allyFigures) {
                                availableFigures[ally] = false;
                                var allyMoveDirections =
                                    figuresController.CalculateFigureMoveDirections(ally);
                                foreach (var allyDirection in allyMoveDirections) {
                                    if (allyDirection.Contains(enemy.position)) {
                                        Debug.Log(ally);
                                        availableFigures[ally] = true;
                                    }
                                    foreach (var allyMovePosition in allyDirection) {
                                        if (enemyDirection.Contains(allyMovePosition)) {
                                            availableFigures[ally] = true;
                                        }
                                    }

                                }
                            }
                            return;
                        }
                    }
                }

                foreach (var figure in figures) {
                    availableFigures[figure] = true;
                }
                fieldRenderers[king.position].material = materials[(king.position.x + king.position.y) % 2];
            }
        }


        private void CheckCheck() {

        }
    }
}
