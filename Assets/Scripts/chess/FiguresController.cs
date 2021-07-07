using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.chess {
    public class FiguresController : MonoBehaviour {

        [SerializeField]
        private LayerMask raycastLayer;
        [SerializeField]
        private List<Material> materials;
        [SerializeField]
        private List<FigureColor> colors;
        [SerializeField]
        private List<GameObject> figures;

        private Dictionary<FigureColor, Material> colorMaterials;

        private void Start() {
            colorMaterials = new Dictionary<FigureColor, Material>();
            for (int i = 0; i < colors.Count && i < materials.Count; i++) {
                colorMaterials.Add(colors[i], materials[i]);
            }
            foreach (var figure in figures) {
                InitializeFigure(figure);
            }
        }

        private void InitializeFigure(GameObject figure) {
            var figureComponent = figure.GetComponent<ChessFigure>();
            GetFigureMovePositions(figureComponent);
            var renderers = figure.GetComponentsInChildren<MeshRenderer>();
            SetMaterials(renderers, colorMaterials[figureComponent.color]);
        }

        private void SetMaterials(MeshRenderer[] renderers, Material material) {
            foreach (var renderer in renderers) {
                renderer.material = material;
            }
        }

        private List<List<Vector3>> GetFigureMovePositions(ChessFigure figure) {
            List<List<Vector3>> moveDirections = new List<List<Vector3>>();
            switch (figure.type) {
                case Figure.Pawn: {
                        break;
                    }
                case Figure.Bishop: {
                        float distance = 8;
                        GetForwardLeftPositions(figure, distance);
                        GetForwardRightPositions(figure, distance);
                        GetBackLeftPositions(figure, distance);
                        GetBackRightPositions(figure, distance);
                        break;
                    }
                case Figure.King: {
                        float distance = 1;
                        GetForwardLeftPositions(figure, distance);
                        GetForwardRightPositions(figure, distance);
                        GetBackLeftPositions(figure, distance);
                        GetBackRightPositions(figure, distance);
                        GetForwardPositions(figure, distance);
                        GetBackPositions(figure, distance);
                        GetLeftPositions(figure, distance);
                        GetRightPositions(figure, distance);
                        break;
                    }
                case Figure.Knight: {

                        break;
                    }
                case Figure.Queen: {
                        float distance = 8;
                        GetForwardLeftPositions(figure, distance);
                        GetForwardRightPositions(figure, distance);
                        GetBackLeftPositions(figure, distance);
                        GetBackRightPositions(figure, distance);
                        GetForwardPositions(figure, distance);
                        GetBackPositions(figure, distance);
                        GetLeftPositions(figure, distance);
                        GetRightPositions(figure, distance);
                        break;
                    }
                case Figure.Rook: {
                        float distance = 8;
                        GetForwardPositions(figure, distance);
                        GetBackPositions(figure, distance);
                        GetLeftPositions(figure, distance);
                        GetRightPositions(figure, distance);
                        break;
                    }
                default: {
                        break;
                    }
            }
            return moveDirections;
        }

        private List<Vector3> GetForwardLeftPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, figure.transform.forward + Vector3.left, distance);
        }

        private List<Vector3> GetForwardRightPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, figure.transform.forward + Vector3.right, distance);
        }

        private List<Vector3> GetBackLeftPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, -figure.transform.forward + Vector3.left, distance);
        }

        private List<Vector3> GetBackRightPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, -figure.transform.forward + Vector3.right, distance);
        }

        private List<Vector3> GetForwardPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, figure.transform.forward, distance);
        }

        private List<Vector3> GetBackPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, -figure.transform.forward, distance);
        }

        private List<Vector3> GetRightPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, figure.transform.right, distance);
        }
        private List<Vector3> GetLeftPositions(ChessFigure figure, float distance) {
            return RaycastPositions(figure.position, -figure.transform.right, distance);
        }

        private List<Vector3> RaycastPositions(Vector3 position, Vector3 direction, float distance) {
            List<Vector3> positions = new List<Vector3>();
            Ray ray = new Ray(position, direction);
            Vector3 rayEnd = ray.origin + ray.direction * distance;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, raycastLayer)) {
                var hitColliderPosition = hitInfo.collider.transform.position;
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("ChessFigure")) {
                    SubdivideVector(ray.origin, hitColliderPosition, ray.direction);
                } else {
                    SubdivideVector(ray.origin, hitColliderPosition - ray.direction, ray.direction);
                }
                SubdivideVector(ray.origin, hitColliderPosition, ray.direction);
            } else {
                SubdivideVector(ray.origin, rayEnd, ray.direction);
            }
            return positions;
        }

        private List<Vector3> SubdivideVector(Vector3 start, Vector3 end, Vector3 divider) {
            List<Vector3> positions = new List<Vector3>();
            Debug.DrawLine(start, end, Color.green, 3);
            return positions;
        }
    }

}

