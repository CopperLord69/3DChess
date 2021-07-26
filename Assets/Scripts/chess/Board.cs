using events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using ev;
using jonson.reflect;
using jonson;
using UnityEngine.SceneManagement;
using chessEngine;

namespace chess {
    public class Board : MonoBehaviour {

        public static bool loadGameOnStart = false;

        public UnityEvent onPawnTransformation;
        public UnityEvent onCheck;
        public UnityEvent onNoCheck;
        public UnityEvent onMate;
        public UnityEvent onStalemate;

        [SerializeField]
        private FigurePicker picker;

        [SerializeField]
        private Material positionMaterial;
        [SerializeField]
        private Material dangerMaterial;

        [SerializeField]
        private Material blackMaterial;

        [SerializeField]
        private Material whiteMaterial;

        [SerializeField]
        private float pickHeight;

        [SerializeField]
        private List<GameObject> prefabs;

        private PieceInfo selectedPiece;
        private List<Vector3> selectedFigureMovePositions;
        private Dictionary<Position, Position> passantPositions;
        private Dictionary<Position, Position> castlingPositions;

        private Token figureSelectionToken = new Token();
        private Token figureMovementToken = new Token();

        [SerializeField]
        private GameObject fieldPrefab;

        [SerializeField]
        private Transform figureParent;

        [SerializeField]
        private Transform fieldParent;
        private List<List<Field>> board;

        private List<FieldInfo> fields;

        private bool isWhiteTurn = true;

        private void Start() {
            selectedFigureMovePositions = new List<Vector3>();
            passantPositions = new Dictionary<Position, Position>();
            board = new List<List<Field>>();
            fields = new List<FieldInfo>();
            if (loadGameOnStart) {
                LoadBoard();
                return;
            }
            for (int i = 0; i < 8; i++) {
                List<Field> row = new List<Field>();
                for (int j = 0; j < 8; j++) {
                    bool isFilled = false;
                    Position position = new Position {
                        x = i,
                        y = j
                    };
                    Figure figure = new Figure() { figureType = FigureType.None };
                    if (j == 1 || j == 6) {
                        figure.figureType = FigureType.Pawn;
                        isFilled = true;
                    } else {
                        if (j == 0 || j == 7) {
                            if (i == 0 || i == 7) {
                                figure.figureType = FigureType.Rook;
                                isFilled = true;
                            }
                            if (i == 1 || i == 6) {
                                figure.figureType = FigureType.Knight;
                                isFilled = true;
                            }
                            if (i == 2 || i == 5) {
                                figure.figureType = FigureType.Bishop;
                                isFilled = true;
                            }
                            if (i == 3) {
                                figure.figureType = FigureType.Queen;
                                isFilled = true;
                            }
                            if (i == 4) {
                                figure.figureType = FigureType.King;
                                isFilled = true;
                            }
                        }
                    }
                    bool isWhite = false;
                    if (j < 3) {
                        isWhite = true;
                    }
                    row.Add(new Field {
                        position = position,
                        figure = figure,
                        isWhite = isWhite,
                        isFilled = isFilled
                    });
                }
                board.Add(row);
            }
            picker.moveEvent.handler.Register(figureMovementToken, MakeFigureTurn);
            picker.pickEvent.handler.Register(figureSelectionToken, SelectFigure);
            GenerateFigures();
        }

        public void TransformFigureInto(GameObject prefab) {

        }

        private void SelectFigure(FigPickEvent e) {
            if (selectedPiece.gameObject != null) {
                var endPos = new Vector3(
                    selectedPiece.gameObject.transform.localPosition.x,
                    1,
                    selectedPiece.gameObject.transform.localPosition.z);
                MoveFigure(selectedPiece.gameObject, endPos);
                DeselectFigure(selectedPiece);
            }
            var pickedFigurePosition = e.figure.transform.localPosition;
            var figurePosition = new Position {
                x = Mathf.RoundToInt(pickedFigurePosition.x),
                y = Mathf.RoundToInt(pickedFigurePosition.z),
            };
            if(board[figurePosition.x][figurePosition.y].isWhite != isWhiteTurn) {
                return;
            }
            var figureObject = e.figure;
            var figureLiftPosition = new Vector3(pickedFigurePosition.x, 2, pickedFigurePosition.z);
            MoveFigure(figureObject, figureLiftPosition);
            var figureMovePositions = Chess.CalculateMovePositions(figurePosition, board);
            selectedFigureMovePositions.Clear();
            foreach (var position in figureMovePositions) {
                selectedFigureMovePositions.Add(new Vector3() {
                    x = position.x,
                    y = 1,
                    z = position.y
                });
            }
            selectedPiece.gameObject = e.figure;
            selectedPiece.figureCollider = e.figure.GetComponent<Collider>();
            selectedPiece.figureCollider.enabled = false;
            ReturnMaterials();
            ColorizeMoveFields();
            ColorizeDangerFields();
        }

        private void MakeFigureTurn(FigMoveEvent e) {
            var figureEndPosition = new Position {
                x = Mathf.RoundToInt(e.position.x),
                y = Mathf.RoundToInt(e.position.z)
            };
            var figureStartPosition = new Position {
                x = Mathf.RoundToInt(selectedPiece.gameObject.transform.localPosition.x),
                y = Mathf.RoundToInt(selectedPiece.gameObject.transform.localPosition.z)
            };
            if (selectedFigureMovePositions.Contains(e.position)) {
                var endField = board[figureEndPosition.x][figureEndPosition.y];
                var figureField = board[figureStartPosition.x][figureStartPosition.y];
                foreach(var row in board) {
                    foreach(var field in row) {
                        field.figure.madeMoveJustNow = false;
                    }
                }
                endField.figure = figureField.figure;
                endField.figure.madeMoveJustNow = true;
                endField.figure.movesCount += 1;
                endField.isFilled = true;
                endField.isWhite = figureField.isWhite;
                figureField.isFilled = false;
                figureField.figure.madeMoveJustNow = false;
                figureField.figure.figureType = FigureType.None;
                MoveFigure(selectedPiece.gameObject, e.position);
                DeselectFigure(selectedPiece);
                ReturnMaterials();
                isWhiteTurn = !isWhiteTurn;
            } else {
                return;
            }
        }

        private void MoveFigure(GameObject figure, Vector3 endPosition) {
            if (figure.gameObject.TryGetComponent(out FigureMover mover)) {
                mover.endPosition = endPosition;
                mover.enabled = true;
            } else {
                var moverComponent = figure.gameObject.AddComponent<FigureMover>();
                moverComponent.endPosition = endPosition;
            }
        }

        private void CheckForCheck() {

        }



        private void ColorizeDangerFields() {

        }

        private void ColorizeMoveFields() {
            int count = -1;
            foreach (var pos in selectedFigureMovePositions) {
                count++;
                var fieldPos = new Vector3(pos.x, 0, pos.z);
                if (count >= fields.Count) {
                    int difference = count - fields.Count + 1;
                    for (int i = difference; i > 0; i--) {
                        var fieldObj = Instantiate(fieldPrefab, fieldParent);
                        fieldObj.transform.localPosition = fieldPos;
                        var fieldInfo = fieldObj.GetComponent<FieldInfo>();
                        fieldInfo.renderer.material = positionMaterial;
                        fields.Add(fieldInfo);
                    }
                } else {
                    fields[count].transform.localPosition = fieldPos;
                }
            }
        }

        private void ReturnMaterials() {
            foreach (var field in fields) {
                field.gameObject.transform.Translate(Vector3.down);
            }
        }

        private void DeselectFigure(PieceInfo piece) {
            piece.figureCollider.enabled = true;
            piece.gameObject = null;
        }




        public void SaveBoard() {

        }

        public void LoadBoard() {

        }

        private void GenerateFigures() {
            foreach (var row in board) {
                foreach (var field in row) {
                    if (field.isFilled) {
                        Material material;
                        if (field.isWhite) {
                            material = whiteMaterial;
                        } else {
                            material = blackMaterial;
                        }
                        var figure = Instantiate(prefabs[(int)field.figure.figureType]);
                        var renderers = figure.GetComponentsInChildren<MeshRenderer>();
                        foreach (var renderer in renderers) {
                            renderer.material = material;
                        }
                        figure.transform.parent = figureParent;
                        figure.transform.localPosition = new Vector3(
                            field.position.x,
                            1,
                            field.position.y
                        );
                    }
                }
            }
        }

        public void ReloadGame() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ExitGame() {
            Application.Quit();
        }
    }
}
