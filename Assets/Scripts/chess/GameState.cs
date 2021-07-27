using chessEngine;
using System.Collections.Generic;

namespace chess {
    public struct GameState {
        public List<List<Field>> board;
        public bool isWhiteTurn;
    }
}