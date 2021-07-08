using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class Board : MonoBehaviour {

        [SerializeField]
        private List<GameObject> fields;
        [SerializeField]
        private List<ChessFigure> figures;

        private FiguresController figuresController;

        private string[][] boardPositions = new string[8][];

        private void Awake() {
            int j = 0;
            int k = 0;
            boardPositions[j] = new string[8];
            boardPositions[j][k] = fields[0].name;
            for(int i = 1; i < fields.Count; i++){
                if(fields[i].name[fields[i].name.Length-1] != fields[i - 1].name[fields[i - 1].name.Length - 1]) {
                    j = 0;
                    k++;
                    boardPositions[k] = new string[8];
                }
                boardPositions[k][j] = fields[i].name;
                j++;
            }

            figuresController = new FiguresController(figures, boardPositions); ;
        }
    }
}