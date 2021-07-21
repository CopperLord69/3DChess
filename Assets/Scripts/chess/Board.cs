using ev;
using events;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace chess {
    public class Board : MonoBehaviour {

        public UnityEvent onPawnTransformation;

        [SerializeField]
        private GameState gameState;

        [SerializeField]
        private FigurePicker picker;

        [SerializeField]
        private List<FigureSet> figuresSets;
        private List<ChessFigure> figures = new List<ChessFigure>();
        private List<ChessFigure> blackFigures = new List<ChessFigure>();
        private List<ChessFigure> whiteFigures = new List<ChessFigure>();
        private List<ChessFigure> kings = new List<ChessFigure>();
        private Dictionary<Vector2Int, ChessFigure> castlingPositions;
        private Dictionary<Vector2Int, ChessFigure> approachPositions;

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
        private GameObject[][] boardObjects = new GameObject[8][];

        

        private Token figureSelectionToken = new Token();
        private Token figureMovementToken = new Token();

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
            castlingPositions = new Dictionary<Vector2Int, ChessFigure>();
            approachPositions = new Dictionary<Vector2Int, ChessFigure>();
            foreach (var figuresSet in figuresSets) {
                figures.AddRange(figuresSet.figures);
                foreach (var figure in figuresSet.figures) {
                    if (figure.color == FigureColor.Black) {
                        blackColliders.Add(figure.gameObject.GetComponent<Collider>());
                        blackFigures.Add(figure);
                    } else {
                        whiteColliders.Add(figure.gameObject.GetComponent<Collider>());
                        whiteFigures.Add(figure);
                    }
                    if (figure.type == Figure.King) {
                        kings.Add(figure);

                    }
                    if (figure.type == Figure.Rook) {
                        Vector2Int position = figure.position;
                        if (figure.position.x == 0) {
                            position.x = 2;
                        } else {
                            position.x = 6;
                        }
                        castlingPositions.Add(position, figure);
                    }
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


            picker.pickEvent.handler.Register(figureSelectionToken, SelectFiugre);
            picker.moveEvent.handler.Register(figureMovementToken, MakeFigureTurn);
        }

         public void TransformFigureInto(GameObject prefab) {
            var newFigure = Instantiate(
                prefab, 
                selectedFigure.transform.position, 
                selectedFigure.transform.rotation);
            var figureComponent = newFigure.GetComponent<ChessFigure>();
            if(selectedFigure.color == FigureColor.Black) {
                figureComponent.color = FigureColor.Black;
                blackFigures.Add(figureComponent);
                blackColliders.Add(figureComponent.gameObject.GetComponent<Collider>());
            } else {
                figureComponent.color = FigureColor.White;
                whiteFigures.Add(figureComponent);
                whiteColliders.Add(figureComponent.gameObject.GetComponent<Collider>());
            }
            figures.Add(figureComponent);
            figureComponent.position = selectedFigure.position;
            newFigure.GetComponent<FigureInitializer>().alreadyHasPosition = true;
            var parent = selectedFigure.gameObject.transform.parent;
            newFigure.transform.parent = parent;
            Destroy(selectedFigure.gameObject);
            CheckForCheck();
            ReturnMaterials();
        }

        private void MakeFigureTurn(FigMoveEvent e) {
            figures.RemoveAll(fig => fig == null);
            blackColliders.RemoveAll(collider => collider == null);
            whiteColliders.RemoveAll(collider => collider == null);
            var figureEndPosition = new Vector2Int(
                Mathf.RoundToInt(e.position.x),
                Mathf.RoundToInt(e.position.z)
                );
            foreach (var direction in selectedFigureMoveDirections) {
                if (direction.Contains(figureEndPosition)) {
                    selectedFigure.position = figureEndPosition;
                    
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
                    var enemies = GetEnemies(selectedFigure.color);
                    foreach (var enemy in enemies) {
                        if (enemy.position == figureEndPosition) {
                            DestroyFigure(enemy);
                            break;
                        }
                    }
                    if (selectedFigure.type == Figure.King) {
                        if (castlingPositions.Keys.Contains(figureEndPosition)) {

                            Vector2Int castlePosition;
                            if (figureEndPosition.x == 5) {
                                castlePosition =
                                    new Vector2Int(figureEndPosition.x + 1, figureEndPosition.y);
                            } else {
                                castlePosition =
                                    new Vector2Int(figureEndPosition.x - 1, figureEndPosition.y);
                            }
                            var castlePosition3 =
                                new Vector3(castlePosition.x, 1, castlePosition.y);
                            castlingPositions[figureEndPosition].position = castlePosition;
                            MoveFigure(castlingPositions[figureEndPosition], castlePosition3);
                        }
                    }
                    if(selectedFigure.type == Figure.Pawn) {
                        if(selectedFigure.position.y == 0 || selectedFigure.position.y == 7) {
                            onPawnTransformation?.Invoke();
                        }
                        if (approachPositions.Keys.Contains(figureEndPosition)) {
                            var figEnd3 = new Vector3(figureEndPosition.x, 1, figureEndPosition.y);
                            MoveFigure(selectedFigure, figEnd3);
                            DestroyFigure(approachPositions[figureEndPosition]);
                            
                        }
                    }
                    foreach(var fig in figures) {
                        fig.madeTurnJustNow = false;
                    }
                    selectedFigure.madeTurnJustNow = true;
                    selectedFigure.movesCount += 1;
                    selectedFigureMoveDirections.Clear();
                    CheckForCheck();
                    MoveFigure(e);
                    return;
                }
            }
        }

        private void CheckForCheck() {
            foreach (var king in kings) {
                if (IsKingInDanger(king)) {
                    HandleCheck();
                    return;
                }
            }
            HandleNoCheck();
        }

        private void DestroyFigure(ChessFigure enemy) {
            figures.Remove(enemy);
            blackFigures.Remove(enemy);
            whiteFigures.Remove(enemy);
            Destroy(enemy.gameObject);
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
                figureObject.transform.localPosition.x,
                pickHeight,
                figureObject.transform.localPosition.z);
            selectedFigure = e.figure;
            MoveFigure(selectedFigure, endPosition);
            selectedFigureMoveDirections.Clear();
            selectedFigureMoveDirections = CalculateMoveDirections(selectedFigure);
            if (selectedFigure.type == Figure.King) {
                if (selectedFigure.movesCount == 0) {

                    foreach (var position in castlingPositions.Keys) {
                        if (castlingPositions[position].movesCount != 0) {
                            continue;
                        }
                        var allies = GetAllies(selectedFigure.color);
                        int rowFigureCount = 0;
                        foreach (var ally in allies) {
                            if (ally.position.y == selectedFigure.position.y) {
                                if (position.x == 2) {
                                    if (ally.position.x > 0 && ally.position.x < 4) {
                                        rowFigureCount++;
                                        break;
                                    }
                                } else {
                                    if (ally.position.x > 4 && ally.position.x < 7) {
                                        rowFigureCount++;
                                        break;
                                    }
                                }
                            }
                        }
                        if (rowFigureCount == 0 && position.y == selectedFigure.position.y) {
                            selectedFigureMoveDirections.Add(new List<Vector2Int> { position });
                        }
                    }
                } else {
                    castlingPositions.Clear();
                }
            }
            if (selectedFigure.type == Figure.Pawn) {
                approachPositions.Clear();
                var enemies = GetEnemies(selectedFigure.color);
                int enemyFirstMoveLine;
                int xOffset;
                if (selectedFigure.color == FigureColor.White) {
                    enemyFirstMoveLine = 4;
                    xOffset = 1;
                } else {
                    enemyFirstMoveLine = 3;
                    xOffset = -1;
                }
                foreach (var enemy in enemies) {
                    if (
                        enemy.position.y == enemyFirstMoveLine 
                        && enemy.position.y == selectedFigure.position.y
                        ) {
                        if (Mathf.Abs(enemy.position.x - selectedFigure.position.x) < 2) {
                            if (
                                enemy.type == Figure.Pawn 
                                && enemy.movesCount == 1 
                                && enemy.madeTurnJustNow
                                ) {
                                var pos = enemy.position;
                                pos.y += xOffset;
                                selectedFigureMoveDirections.Add(new List<Vector2Int> { pos });
                                approachPositions.Add(pos, enemy);
                            }
                        }
                    }
                }
            }
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
            MoveFigure(selectedFigure, endPosition);
            ReturnMaterials();
        }

        private void MoveFigure(FigMoveEvent e) {
            MoveFigure(selectedFigure, e.position);
        }

        private void MoveFigure(ChessFigure figure, Vector3 endPosition) {
            if (figure.gameObject.TryGetComponent(out FigureMover mover)) {
                mover.endPosition = endPosition;
            } else {
                var moverComponent = figure.gameObject.AddComponent<FigureMover>();
                moverComponent.endPosition = endPosition;
            }
        }

        private void ReturnMaterials() {
            for (int i = 0; i < fieldRenderers.Count; i++) {
                fieldRenderers.ElementAt(i).Value.material = materials[i % 2];
            }

            foreach (var king in kings) {
                if (IsKingInDanger(king)) {
                    fieldRenderers[king.position].material = dangerMaterial;
                }
            }
        }

        private List<List<Vector2Int>> CalculateMoveDirections(ChessFigure figure) {
            var allyKing = FindKingWithColor(figure.color);
            var impossiblePositions = new List<Vector2Int>();
            Vector2Int initialPosition = figure.position;
            var figureMoveDirections = figuresController.CalculateFigureMoveDirections(figure);
            foreach (var direction in figureMoveDirections) {
                foreach (var position in direction) {
                    figure.position = position;
                    if (IsKingInDanger(allyKing)) {
                        impossiblePositions.Add(position);
                    }
                }
                foreach (var position in impossiblePositions) {
                    direction.Remove(position);
                }
            }
            figure.position = initialPosition;
            figureMoveDirections.RemoveAll(dir => dir.Count == 0);
            return figureMoveDirections;
        }

        private bool IsKingInDanger(ChessFigure king) {
            List<ChessFigure> enemies = GetEnemies(king.color);
            foreach (var enemy in enemies) {
                var enemyMoveDirecitons = figuresController.CalculateFigureMoveDirections(enemy);
                foreach (var enemyDirection in enemyMoveDirecitons) {
                    if (enemyDirection.Contains(king.position)) {
                        return true;
                    }
                }
            }
            return false;
        }

        private void HandleNoCheck() {
            foreach (var king in kings) {
                if (NoAvailableMoves(king.color)) {
                    print("stalemate");
                    return;
                }
            }
        }

        private void HandleCheck() {
            Debug.Log("check");
            CheckForMate();
        }

        private void CheckForMate() {
            foreach (var king in kings) {
                if (IsKingInDanger(king)) {
                    if (NoAvailableMoves(king.color)) {
                        figureMovementToken.Unsubscribe();
                        figureSelectionToken.Unsubscribe();
                        print("mate");
                        return;
                    }
                }
            }
        }

        private bool NoAvailableMoves(FigureColor color) {
            var allies = GetAllies(color);
            foreach (var ally in allies) {
                var allyMoveDeirections = CalculateMoveDirections(ally);
                if (allyMoveDeirections.Count != 0) {
                    return false;
                }
            }
            return true;
        }

        private ChessFigure FindKingWithColor(FigureColor color) {
            var king = figures.Find(figure => figure.type == Figure.King && figure.color == color);
            return king;
        }

        private List<ChessFigure> GetAllies(FigureColor color) {
            if (color == FigureColor.Black) {
                return blackFigures;
            } else {
                return whiteFigures;
            }
        }

        private List<ChessFigure> GetEnemies(FigureColor color) {
            if (color == FigureColor.White) {
                return blackFigures;
            } else {
                return whiteFigures;
            }
        }

       
    }
}
