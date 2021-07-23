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
        private FigureSet figureSet;
        private List<PieceInfo> pieceInfos;

        [SerializeField]
        private Material positionMaterial;
        [SerializeField]
        private Material dangerMaterial;

        [SerializeField]
        private float pickHeight;

        [SerializeField]
        private List<GameObject> prefabs;

        private PieceInfo selectedPiece;
        private List<Vector3> selectedFigureMovePositions;
        private Dictionary<Position, Position> passantPositions;
        private Dictionary<Position, Position> castlingPositions;

        private FigureColor currentPlayer = FigureColor.White;

        private Token figureSelectionToken = new Token();
        private Token figureMovementToken = new Token();

        [SerializeField]
        private GameObject fieldPrefab;
        [SerializeField]
        private Transform fieldParent;
        private List<Field> fields;

        private Chess chess;

        private bool isWhiteTurn = true;

        private void Start() {
            selectedFigureMovePositions = new List<Vector3>();
            pieceInfos = new List<PieceInfo>();
            passantPositions = new Dictionary<Position, Position>();
            foreach (var figure in figureSet.figures) {
                var figureCollider = figure.GetComponent<Collider>();
                var figT = figure.GetComponent<FigureType>();
                if (figT.color != FigureColor.White) {
                    figureCollider.enabled = false;
                }
                pieceInfos.Add(new PieceInfo {
                    piece = figure,
                    figureCollider = figureCollider,
                    color = figT.color,
                    type = figT.type
                });
            }
            fields = new List<Field>();
            chess = new Chess();
            picker.pickEvent.handler.Register(figureSelectionToken, SelectFiugre);
            picker.moveEvent.handler.Register(figureMovementToken, MakeFigureTurn);
        }

        public void TransformFigureInto(GameObject prefab) {
            var newPiece = Instantiate(
                prefab,
                selectedPiece.piece.transform.position,
                selectedPiece.piece.transform.rotation
            );
            pieceInfos.Remove(selectedPiece);
            var figureType = prefab.GetComponent<FigureType>();
            selectedPiece.type = figureType.type;
            Destroy(selectedPiece.piece.gameObject);
            var parent = selectedPiece.piece.transform.parent;
            newPiece.transform.parent = parent;
            var piecePosition = selectedPiece.piece.transform.localPosition;
            selectedPiece.piece = newPiece;
            selectedPiece.figureCollider = newPiece.GetComponent<Collider>();
            selectedPiece.color = figureType.color;
            newPiece.GetComponent<FigureInitializer>().alreadyHasPosition = true;
            Position figurePosition = new Position() {
                x = Mathf.RoundToInt(piecePosition.x),
                y = Mathf.RoundToInt(piecePosition.z),
            };

            Position newPosition = new Position() {
                x = Mathf.RoundToInt(newPiece.transform.localPosition.x),
                y = Mathf.RoundToInt(newPiece.transform.localRotation.z),
            };
            chess.TransformFigure(figurePosition, figureType.type);
            chess.SetFigurePosition(figurePosition, newPosition);
            pieceInfos.Add(selectedPiece);
            CheckForCheck();
            ReturnMaterials();
        }

        private void SelectFiugre(FigPickEvent e) {
            var pickedFigurePosition = e.figure.transform.localPosition;
            var figurePosition = new Position {
                x = Mathf.RoundToInt(pickedFigurePosition.x),
                y = Mathf.RoundToInt(pickedFigurePosition.z),
            };
            var sel = chess.GetFigureOnPosition(figurePosition);
            if (sel.IsSome()) {
                foreach (var pInfo in pieceInfos) {
                    var distance =
                        Vector3.Distance(pInfo.piece.transform.localPosition, pickedFigurePosition);
                    if (distance < 0.1f) {
                        if (selectedPiece.piece != null) {
                            DeselectFigure(selectedPiece.piece);
                        }
                        selectedPiece = pInfo;
                        
                        var positions = chess.CalculateMovePositions(figurePosition);
                        selectedFigureMovePositions.Clear();
                        foreach (var pos in positions) {
                            selectedFigureMovePositions.Add(new Vector3(pos.x, 1, pos.y));
                        }
                        if (pInfo.type == Figure.Pawn) {
                            passantPositions = chess.GetPawnElPassants(figurePosition);
                        }
                        if(pInfo.type == Figure.King) {
                            castlingPositions = chess.GetCastlingPositions(figurePosition);
                        }
                        Vector3 moveUpPosition = new Vector3(
                            selectedPiece.piece.transform.localPosition.x,
                            pickHeight,
                            selectedPiece.piece.transform.localPosition.z);
                        MoveFigure(selectedPiece.piece, moveUpPosition);
                        break;
                    }
                }
            }
            ColorizeMoveFields();
            ColorizeDangerFields();
        }

        private void MakeFigureTurn(FigMoveEvent e) {
            if (selectedPiece.piece == null) {
                return;
            }
            var figureEndPosition = new Position {
                x = Mathf.RoundToInt(e.position.x),
                y = Mathf.RoundToInt(e.position.z)
            };
            var piecePos = selectedPiece.piece.transform.localPosition;
            var figureStartPosition = new Position {
                x = Mathf.RoundToInt(piecePos.x),
                y = Mathf.RoundToInt(piecePos.z)
            };
            foreach (var position in selectedFigureMovePositions) {
                var distance = Vector3.Distance(position, e.position);
                if (distance < 0.2f) {
                    ReturnMaterials();
                    var collidingEnemy = chess.GetFigureOnPosition(figureEndPosition);
                    if (collidingEnemy.IsSome()) {
                        var collidingPosition3 = new Vector3 {
                            x = collidingEnemy.Peel().position.x,
                            y = 1,
                            z = collidingEnemy.Peel().position.y
                        };
                        foreach (var pInfo in pieceInfos) {
                            distance = Vector3.Distance(
                                pInfo.piece.transform.localPosition, 
                                collidingPosition3
                            );
                            if (distance < 0.2f) {
                                Destroy(pInfo.piece);
                                chess.DeleteFigureWithPosition(collidingEnemy.Peel().position);
                                pieceInfos.Remove(pInfo);
                                break;
                            }
                        }
                    }
                    if(selectedPiece.type == Figure.Pawn) {
                        if (figureEndPosition.y > 6 ||
                            figureEndPosition.y < 1) { 
                            onPawnTransformation?.Invoke();
                        }
                    }
                    chess.SetFigurePosition(figureStartPosition, figureEndPosition);
                    foreach (var passantPos in passantPositions.Values) {
                        var position3 = new Vector3() {
                            x = passantPos.x,
                            y = 1,
                            z = passantPos.y
                        };
                        foreach(var pieceInfo in pieceInfos) {
                            var dist = Vector3.Distance(
                                pieceInfo.piece.transform.localPosition, 
                                position3
                            );
                            if(dist < 0.2f) {
                                Destroy(pieceInfo.piece);
                                pieceInfos.Remove(pieceInfo);
                                chess.DeleteFigureWithPosition(passantPos);
                                break;
                            }
                        }
                    }
                    CheckForCheck();
                    MoveFigure(selectedPiece.piece, e.position);
                    isWhiteTurn = !isWhiteTurn;
                    foreach (var pInfo in pieceInfos) {
                        bool figureIsWhite = pInfo.color == FigureColor.White;
                        if (figureIsWhite == isWhiteTurn) {
                            pInfo.figureCollider.enabled = true;
                        } else {
                            pInfo.figureCollider.enabled = false;
                        }
                    }
                    return;
                }
            }
        }

        private void CheckForCheck() {
            if (chess.CheckForCheck()) {
                onCheck?.Invoke();
                if (chess.CheckForMate()) {
                    figureMovementToken.Cancel();
                    figureSelectionToken.Cancel();
                    onMate?.Invoke();
                    return;
                }
            } else {
                onNoCheck?.Invoke();
            }
            if (chess.CheckForStalemate()) {
                figureMovementToken.Cancel();
                figureSelectionToken.Cancel();
                onStalemate?.Invoke();
            }
        }



        private void ColorizeDangerFields() {
            var positions = new List<Position>();
            var dangerPositions = new List<Vector3>();
            foreach (var pos in selectedFigureMovePositions) {
                positions.Add(new Position {
                    x = Mathf.RoundToInt(pos.x),
                    y = Mathf.RoundToInt(pos.z)
                });
            }
            foreach (var position in chess.GetDangerPositions(positions)) {
                dangerPositions.Add(new Vector3 {
                    x = position.x, y = 0, z = position.y
                });
            }
            ColorizeFields(dangerPositions, dangerMaterial);
        }

        private void ColorizeFields(List<Vector3> fieldPositions, Material material) {
            int count = 0;
            foreach (var position in fieldPositions) {
                Vector3 pos = new Vector3(
                    position.x,
                    0,
                    position.z);
                count++;
                if (count < fields.Count) {
                    fields[count].transform.localPosition = pos;
                    fields[count].renderer.material = material;
                } else {
                    var fieldObject = Instantiate(fieldPrefab);
                    fieldObject.transform.parent = fieldParent;
                    fieldObject.transform.localPosition = pos;
                    var field = fieldObject.GetComponent<Field>();
                    field.renderer.material = material;
                    fields.Add(field);

                }
            }
        }

        private void ColorizeMoveFields() {
            ColorizeFields(selectedFigureMovePositions, positionMaterial);
        }

        private void ReturnMaterials() {
            foreach (var field in fields) {
                field.transform.localPosition = new Vector3(0, -1, 0);
            }
            var kingInDangerPosition = chess.GetKingInDangerPosition();
            if (kingInDangerPosition.IsSome()) {
                fields[0].renderer.material = dangerMaterial;
            }
        }

        private void DeselectFigure(GameObject figureObject) {
            var endPosition = new Vector3(
                figureObject.transform.localPosition.x,
                1,
                figureObject.transform.localPosition.z);
            MoveFigure(selectedPiece.piece, endPosition);
            ReturnMaterials();
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



        private List<PieceInfo> RemoveNullPieces(List<PieceInfo> figures) {
            List<PieceInfo> nullFigures = new List<PieceInfo>();
            foreach (var figure in figures) {
                if (figure.piece == null) {
                    nullFigures.Add(figure);
                }
            }
            foreach (var nullFigure in nullFigures) {
                figures.Remove(nullFigure);
            }
            return figures;
        }

        public void SaveBoard(GameObject stateObject) {
            pieceInfos = RemoveNullPieces(pieceInfos);
            var state = stateObject.GetComponent<GameState>();
            state.figures = chess.GetFigures();
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
            if (selectedPiece.piece != null) {
                DeselectFigure(selectedPiece.piece);
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
            pieceInfos.Clear();
            currentPlayer = state.currentPlayer;
            chess = new Chess(state.figures);
            foreach (var figure in state.figures) {
                var figureObject = Instantiate(prefabs[(int)figure.type]);
                figureObject.transform.localPosition =
                    new Vector3(figure.position.x, 1, figure.position.y);
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
