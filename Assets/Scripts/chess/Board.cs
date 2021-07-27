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
using option;

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
        private List<GameObject> prefabs;

        private PieceInfo selectedPiece;
        private List<Vector3> selectedFigureMovePositions;

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
        private List<PieceInfo> pieces;

        private bool isWhiteTurn = true;

        private void Start() {
            pieces = new List<PieceInfo>();
            selectedFigureMovePositions = new List<Vector3>();
            board = new List<List<Field>>();
            fields = new List<FieldInfo>();
            picker.moveEvent.handler.Register(figureMovementToken, MakeFigureTurn);
            picker.pickEvent.handler.Register(figureSelectionToken, SelectFigure);
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
                    if (j < 3) {
                        figure.isWhite = true;
                    }
                    row.Add(new Field {
                        position = position,
                        figure = figure,
                        isFilled = isFilled
                    });
                }
                board.Add(row);
            }
            GenerateFigures();
        }

        public void TransformFigureInto(int type) {
            Vector3 position = selectedPiece.gameObject.transform.localPosition;
            int x = Mathf.RoundToInt(selectedPiece.gameObject.transform.localPosition.x);
            int y = Mathf.RoundToInt(selectedPiece.gameObject.transform.localPosition.z);
            board[x][y].figure.figureType = (FigureType)type;
            var newFigure = Instantiate(prefabs[type], figureParent);
            newFigure.transform.localPosition = position;
            PieceInfo piece = new PieceInfo() {
                gameObject = newFigure,
                figureCollider = newFigure.GetComponent<Collider>()
            };
            Destroy(selectedPiece.gameObject);
            pieces.Remove(selectedPiece);
            pieces.Add(piece);
        }

        private void SelectFigure(FigPickEvent e) {
            if (selectedPiece.gameObject != null) {
                var endPos = new Vector3(
                    selectedPiece.gameObject.transform.localPosition.x,
                    1,
                    selectedPiece.gameObject.transform.localPosition.z);
                MoveFigure(selectedPiece.gameObject, endPos);
                selectedFigureMovePositions.Clear();
                ReturnMaterials();
            }
            var pickedFigurePosition = e.figure.transform.localPosition;
            var figurePosition = new Position {
                x = Mathf.RoundToInt(pickedFigurePosition.x),
                y = Mathf.RoundToInt(pickedFigurePosition.z),
            };
            if (board[figurePosition.x][figurePosition.y].figure.isWhite != isWhiteTurn) {
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
            foreach (var piece in pieces) {
                if (piece.gameObject == e.figure) {
                    selectedPiece = piece;
                    break;
                }
            }
            ReturnMaterials();
            ColorizeFields();
        }

        private void MakeFigureTurn(FigMoveEvent e) {
            if (selectedPiece.gameObject == null) {
                return;
            }
            var figureEndPosition = new Position {
                x = Mathf.RoundToInt(e.position.x),
                y = Mathf.RoundToInt(e.position.z)
            };
            var figureStartPosition = new Position {
                x = Mathf.RoundToInt(selectedPiece.gameObject.transform.localPosition.x),
                y = Mathf.RoundToInt(selectedPiece.gameObject.transform.localPosition.z)
            };
            if (!selectedFigureMovePositions.Contains(e.position)) {
                return;
            }
            var endField = board[figureEndPosition.x][figureEndPosition.y];
            var figureField = board[figureStartPosition.x][figureStartPosition.y];

            if (endField.isFilled) {
                if (endField.figure.isWhite != figureField.figure.isWhite) {
                    DestroyPiece(e.position);
                }
            }

            if (figureField.figure.figureType == FigureType.Pawn) {
                var passants = Chess.GetPawnEnPassants(figureField.position, board);
                foreach (var pass in passants) {
                    if (figureEndPosition == pass.Key) {
                        var enemyPosition = new Vector3(pass.Value.x, 1, pass.Value.y);
                        DestroyPiece(enemyPosition);

                    }
                }
                if(figureEndPosition.y > 6 || figureEndPosition.y < 1) {
                    onPawnTransformation?.Invoke();
                }
            }
            if (figureField.figure.figureType == FigureType.King) {
                var castlings = Chess.GetCastlingPositions(figureField.position, board);
                foreach (var castling in castlings) {
                    if (castling.Key == figureEndPosition) {
                        Vector3 rookPos = new Vector3(castling.Value.x, 1, castling.Value.y);
                        foreach (var piece in pieces) {
                            if (Vector3.Distance(piece.gameObject.transform.localPosition, rookPos) < 0.2) {
                                Vector3 rookEndPosition;
                                if (castling.Key.x == 2) {
                                    rookEndPosition = new Vector3(castling.Key.x + 1, 1, castling.Key.y);
                                } else {
                                    rookEndPosition = new Vector3(castling.Key.x - 1, 1, castling.Key.y);
                                }
                                board[castling.Value.x][castling.Value.y].isFilled = false;
                                board[castling.Value.x][castling.Value.y].figure.figureType
                                    = FigureType.None;
                                Position rookEnd = new Position {
                                    x = (int)rookEndPosition.x,
                                    y = (int)rookEndPosition.z
                                };
                                board[rookEnd.x][rookEnd.y].figure.figureType = FigureType.Rook;
                                board[rookEnd.x][rookEnd.y].isFilled = true;
                                MoveFigure(piece.gameObject, rookEndPosition);
                                break;
                            }
                        }

                    }
                }
            }
            foreach (var row in board) {
                foreach (var field in row) {
                    field.figure.madeMoveJustNow = false;
                }
            }
            endField.figure = figureField.figure;
            endField.figure.madeMoveJustNow = true;
            endField.figure.movesCount = figureField.figure.movesCount + 1;
            endField.isFilled = true;
            figureField.isFilled = false;
            figureField.figure.madeMoveJustNow = false;
            figureField.figure.figureType = FigureType.None;
            figureField.figure.movesCount = 0;
            MoveFigure(selectedPiece.gameObject, e.position);
            selectedFigureMovePositions.Clear();
            ReturnMaterials();
            isWhiteTurn = !isWhiteTurn;
            ToggleColliders();
            CheckSituation();
        }

        private void CheckSituation() {
            var gameSituation = Chess.GetGameSituation(isWhiteTurn, board);
            switch (gameSituation) {
                case GameSituation.Nothing: {
                        onNoCheck?.Invoke();
                        break;
                    }
                case GameSituation.Check: {
                        onCheck?.Invoke();
                        break;
                    }
                case GameSituation.Stalemate: {
                        onStalemate?.Invoke();
                        break;
                    }
                case GameSituation.Mate: {
                        onMate?.Invoke();
                        break;
                    }
            }
        }

        private void DestroyPiece(Vector3 position) {
            foreach (var piece in pieces) {
                if (Vector3.Distance(position, piece.gameObject.transform.localPosition) < 0.2f) {
                    Destroy(piece.gameObject);
                    pieces.Remove(piece);
                    board[(int)position.x][(int)position.z].figure.figureType = FigureType.None;
                    board[(int)position.x][(int)position.z].isFilled = false;
                    break;
                }
            }
        }

        private void ToggleColliders() {
            foreach (var piece in pieces) {
                int x = Mathf.RoundToInt(piece.gameObject.transform.localPosition.x);
                int y = Mathf.RoundToInt(piece.gameObject.transform.localPosition.z);
                var field = board[x][y];
                bool isRightColor = field.figure.isWhite == isWhiteTurn;
                piece.figureCollider.enabled = isRightColor;
            }
        }
        private void MoveFigure(GameObject figure, Vector3 endPosition) {
            if (figure.TryGetComponent(out FigureMover mover)) {
                mover.endPosition = endPosition;
                mover.enabled = true;
            } else {
                var moverComponent = figure.AddComponent<FigureMover>();
                moverComponent.endPosition = endPosition;
            }
        }

        private void ColorizeFields() {
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

                        fields.Add(fieldInfo);
                    }
                }
                fields[count].transform.localPosition = fieldPos;
                if (board[(int)pos.x][(int)pos.z].isFilled) {
                    fields[count].renderer.material = dangerMaterial;
                } else {

                    fields[count].renderer.material = positionMaterial;
                }
            }
        }

        private void ReturnMaterials() {
            foreach (var field in fields) {
                field.gameObject.transform.localPosition = new Vector3(
                    field.gameObject.transform.localPosition.x,
                    -0.2f,
                    field.gameObject.transform.localPosition.y);
            }
        }

        public void SaveBoard() {
            var state = new GameState {
                board = board,
                isWhiteTurn = isWhiteTurn
            };
            var result = Reflect.ToJSON(state, true);
            string resultStr = Jonson.Generate(result);
            var path = Application.persistentDataPath + "/save.txt";
            if (!File.Exists(path)) {
                var file = File.Create(path);
                file.Close();
            }
            var writer = new StreamWriter(path, false);
            writer.Write(resultStr);
            writer.Close();
        }

        public void LoadBoard() {
            if (selectedPiece.gameObject != null) {
                selectedPiece.gameObject = null;
            }

            string path = Application.persistentDataPath + "/save.txt";
            if (!File.Exists(path)) {
                return;
            }
            var reader = new StreamReader(path);
            string readResult = reader.ReadToEnd();
            reader.Close();
            var state = new GameState();
            Result<JSONType, JSONError> stateRes = Jonson.Parse(readResult, 1024);
            if (stateRes.IsErr()) {
                return;
            }
            state = Reflect.FromJSON(state, stateRes.AsOk());
            isWhiteTurn = state.isWhiteTurn;
            board = state.board;
            foreach (var piece in pieces) {
                Destroy(piece.gameObject);
            }
            pieces.Clear();
            GenerateFigures();
            ReturnMaterials();
            CheckSituation();
        }

        private void GenerateFigures() {
            foreach (var row in board) {
                foreach (var field in row) {
                    if (field.isFilled) {
                        if (field.figure.figureType == FigureType.None) {
                            continue;
                        }
                        Material material;
                        if (field.figure.isWhite) {
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
                        PieceInfo piece = new PieceInfo() {
                            gameObject = figure,
                            figureCollider = figure.GetComponent<Collider>()
                        };
                        pieces.Add(piece);
                    }
                }
            }
            ToggleColliders();
        }

        public void ReloadGame() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ExitGame() {
            Application.Quit();
        }
    }
}
