using Assets.Scripts.chess;
using System.Collections.Generic;

public struct GameState 
{
    public FigureColor currentPlayer;
    public List<ChessFigure> blackFigures;
    public List<ChessFigure> whiteFigures;
    public int movesWithoutKills;
}
