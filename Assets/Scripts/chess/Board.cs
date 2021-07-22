using events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using option;
using ev;
using jonson.reflect;
using jonson;
using UnityEngine.SceneManagement;

namespace chess {
    public class Board : MonoBehaviour {

        public UnityEvent onPawnTransformation;
        public UnityEvent onCheck;
        public UnityEvent onNoCheck;
        public UnityEvent onMate;
        public UnityEvent onStalemate;

        [SerializeField]
        private FigurePicker picker;

        [SerializeField]
        private FigureSet figuresSet;
        private List<ChessFigure> figures;
        private List<ChessFigure> blackFigures;
        private List<ChessFigure> whiteFigures;
        private List<ChessFigure> kings;
        private Dictionary<Position, ChessFigure> castlingPositions;
        private Dictionary<Position, ChessFigure> approachPositions;

        private List<Collider> blackColliders;
        private List<Collider> whiteColliders;

        [SerializeField]
        private List<FigureColor> colors;
        [SerializeField]
        private List<Material> materials;
        [SerializeField]
        private Material positionMaterial;
        [SerializeField]
        private Material dangerMaterial;

        [SerializeField]
        private float pickHeight;

        [SerializeField]
        private List<GameObject> prefabs;

        private ChessFigure selectedFigure;
        private List<List<Position>> selectedFigureMoveDirections;
        private GameObject[][] boardObjects = new GameObject[8][];

        private FigureColor currentPlayer = FigureColor.White;

        private Token figureSelectionToken = new Token();
        private Token figureMovementToken = new Token();

        [SerializeField]
        private GameObject fieldPrefab;
        [SerializeField]
        private Transform fieldParent;
        private List<Field> fields;

        private void Start() {
            blackColliders = new List<Collider>();
            whiteColliders = new List<Collider>();
            fields = new List<Field>();
            selectedFigureMoveDirections = new List<List<Position>>();
            castlingPositions = new Dictionary<Position, ChessFigure>();
            approachPositions = new Dictionary<Position, ChessFigure>();
            blackFigures = new List<ChessFigure>();
            whiteFigures = new List<ChessFigure>();
            kings = new List<ChessFigure>();
            figures = new List<ChessFigure>();
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
            }

            var figureMaterials = new Dictionary<FigureColor, Material>();
            for (int k = 0; k < colors.Count; k++) {
                figureMaterials.Add(colors[k], materials[k]);
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
            if (selectedFigure.color == FigureColor.Black) {
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
            figures = Chess.RemoveNullFigures(figures);
            blackColliders.RemoveAll(collider => collider == null);
            whiteColliders.RemoveAll(collider => collider == null);
            var figureEndPosition = new Position() {
                x = Mathf.RoundToInt(e.position.x),
                y = Mathf.RoundToInt(e.position.z)
            };
            foreach (var direction in selectedFigureMoveDirections) {
                if (direction.Contains(figureEndPosition)) {
                    selectedFigure.position = figureEndPosition;

                    bool blackCollidersEnabled;
                    bool whiteCollidersEnabled;
                    if (currentPlayer == FigureColor.Black) {
                        blackCollidersEnabled = false;
                        whiteCollidersEnabled = true;
                        currentPlayer = FigureColor.White;
                    } else {
                        blackCollidersEnabled = true;
                        whiteCollidersEnabled = false;
                        currentPlayer = FigureColor.Black;
                    }
                    foreach (var collider in whiteColliders) {
                        collider.enabled = whiteCollidersEnabled;
                    }
                    foreach (var collider in blackColliders) {
                        collider.enabled = blackCollidersEnabled;
                    }
                    ReturnMaterials();
                    var collidingEnemy = Chess.GetCollidingEnemy(selectedFigure, figures);
                    if (collidingEnemy.IsSome()) {
                        Destroy(collidingEnemy.Peel().gameObject);
                    }
                    if (selectedFigure.type == Figure.King) {
                        foreach (var castlingPos in castlingPositions.Keys) {
                            if (Position.AreSame(castlingPos, figureEndPosition)) {
                                Position castlePosition;
                                if (figureEndPosition.x == 6) {
                                    castlePosition =
                                        new Position(figureEndPosition.x - 1, figureEndPosition.y);
                                } else {
                                    castlePosition =
                                        new Position(figureEndPosition.x + 1, figureEndPosition.y);
                                }
                                var castlePosition3 =
                                    new Vector3(castlePosition.x, 1, castlePosition.y);
                                castlingPositions[figureEndPosition].position = castlePosition;
                                MoveFigure(castlingPositions[figureEndPosition], castlePosition3);
                                break;
                            }
                        }
                    }
                    if (selectedFigure.type == Figure.Pawn) {
                        if (selectedFigure.position.y == 0 || selectedFigure.position.y == 7) {
                            onPawnTransformation?.Invoke();
                        }
                        foreach (var approachPos in approachPositions.Keys) {
                            if (Position.AreSame(approachPos, figureEndPosition)) {
                                var figEnd3 = new Vector3(figureEndPosition.x, 1, figureEndPosition.y);
                                MoveFigure(selectedFigure, figEnd3);
                                DestroyFigure(approachPositions[figureEndPosition]);
                                break;
                            }
                        }
                    }
                    foreach (var fig in figures) {
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
            if (Chess.CheckForCheck(figures)) {
                onCheck?.Invoke();
                if (Chess.CheckForMate(figures)) {
                    figureMovementToken.Cancel();
                    figureSelectionToken.Cancel();
                    onMate?.Invoke();
                    return;
                }
            } else {
                onNoCheck?.Invoke();
            }
            foreach (var king in kings) {
                if (Chess.NoAvailableMoves(king.color, figures)) {
                    figureMovementToken.Cancel();
                    figureSelectionToken.Cancel();
                    onStalemate?.Invoke();
                }
            }
        }

        private void DestroyFigure(ChessFigure enemy) {
            figures.Remove(enemy);
            blackFigures.Remove(enemy);
            whiteFigures.Remove(enemy);
            Destroy(enemy.gameObject);
        }

        private void SelectFiugre(FigPickEvent e) {
            var figure = e.figure;
            if (figure.color != currentPlayer) {
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
            selectedFigureMoveDirections = Chess.CalculateMoveDirections(selectedFigure, figures);
            foreach (var selectedPosition in selectedFigureMoveDirections) {
                foreach (var p in selectedPosition) {
                    Debug.DrawRay(new Vector3(p.x, 0, p.y), Vector3.up, Color.green, 4);
                }
            }
            if (selectedFigure.type == Figure.King) {
                castlingPositions = Chess.GetCastlingPositions(selectedFigure, figures);
                foreach (var position in castlingPositions.Keys) {
                    selectedFigureMoveDirections.Add(new List<Position> { position });
                }
            }
            if (selectedFigure.type == Figure.Pawn) {
                approachPositions = Chess.GetPawnElPassants(figure, figures);
                foreach (var position in approachPositions.Keys) {
                    selectedFigureMoveDirections.Add(new List<Position> { position });
                }
            }
            ColorizeMoveFields();
            ColorizeDangerFields();
        }

        private void ColorizeDangerFields() {
            foreach (var direction in selectedFigureMoveDirections) {
                foreach (var otherFigure in figures) {
                    if (direction.Contains(otherFigure.position)) {
                        if (otherFigure.color != currentPlayer) {
                            var position = new Vector3(
                                otherFigure.position.x,
                                0,
                                otherFigure.position.y);
                            foreach (var field in fields) {
                                if (position == field.transform.localPosition) {
                                    field.renderer.material = dangerMaterial;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ColorizeMoveFields() {
            int count = 0;
            foreach (var direction in selectedFigureMoveDirections) {
                for (int i = 0; i < direction.Count; i++) {
                    Vector3 position = new Vector3(direction[i].x, 0, direction[i].y);
                    count++;
                    if (count < fields.Count) {
                        fields[count].transform.localPosition = position;
                        fields[count].renderer.material = positionMaterial;
                    } else {
                        var fieldObject = Instantiate(fieldPrefab);
                        fieldObject.transform.parent = fieldParent;
                        fieldObject.transform.localPosition = position;
                        var field = fieldObject.GetComponent<Field>();
                        field.renderer.material = positionMaterial;
                        fields.Add(field);

                    }
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
            foreach (var field in fields) {
                field.transform.localPosition = new Vector3(0, -1, 0);
            }
            foreach (var king in kings) {
                if (Chess.IsKingInDanger(king, figures)) {

                }
            }
        }

        public void SaveBoard(GameObject stateObject) {
            figures = Chess.RemoveNullFigures(figures);
            var state = stateObject.GetComponent<GameState>();
            state.figures = figures;
            state.currentPlayer = currentPlayer;
            var result = Reflect.ToJSON(state, true);
            string resultStr = Jonson.Generate(result);
            var path = Application.persistentDataPath + "/save.txt";
            if (!File.Exists(path)) {
                var file = File.Create(path);
                file.Close();
            }
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(resultStr);
            writer.Close();
        }

        public void LoadBoard() {
            if (selectedFigure != null) {
                DeselectFigure(selectedFigure.gameObject);
            }
            string path = Application.persistentDataPath + "/save.txt";
            if (!File.Exists(path)) {
                return;
            }
            StreamReader reader = new StreamReader(path);
            string readResult = reader.ReadToEnd();
            reader.Close();
            GameState state = new GameState();
            Result<JSONType, JSONError> stateRes = Jonson.Parse(readResult, 1024);
            if (stateRes.IsErr()) {
                return;
            }
            state = Reflect.FromJSON(state, stateRes.AsOk());
            var figs = new List<ChessFigure>();
            foreach (var figure in figures) {
                figs.Add(figure);
            }
            foreach (var figure in figs) {
                DestroyFigure(figure);
            }
            figures.Clear();
            blackColliders.Clear();
            blackFigures.Clear();
            whiteFigures.Clear();
            whiteColliders.Clear();
            kings.Clear();
            currentPlayer = state.currentPlayer;
            foreach (var figure in state.figures) {
                var figureObject = Instantiate(prefabs[(int)figure.type]);
                var figureObjectComponent = figureObject.GetComponent<ChessFigure>();
                figureObjectComponent.color = figure.color;
                figureObjectComponent.type = figure.type;
                figureObjectComponent.position = figure.position;
                figureObjectComponent.movesCount = figure.movesCount;
                figureObject.transform.parent = figuresSet.transform;
                figureObject.transform.localPosition =
                    new Vector3(figure.position.x, 1, figure.position.y);
                var collider = figureObject.GetComponent<Collider>();
                figures.Add(figureObjectComponent);
                if (figureObjectComponent.type == Figure.King) {
                    kings.Add(figureObjectComponent);
                }
                if (figureObjectComponent.color == FigureColor.Black) {
                    blackColliders.Add(collider);
                    blackFigures.Add(figureObjectComponent);
                } else {
                    whiteColliders.Add(collider);
                    whiteFigures.Add(figureObjectComponent);
                    figureObject.transform.Rotate(0, 180, 0);
                }
            }
            CheckForCheck();
        }

        public void ReloadGame() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ExitGame() {
            Application.Quit();
        }
    }
}
