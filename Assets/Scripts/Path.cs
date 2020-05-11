using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public int cost;
    public Vector2[] moves;

    public Path(Vector2[] previousMoves, Vector2 currentMove, int cost)
    {
        this.cost = cost;

        if (cost == 0)
        {
            moves = new Vector2[0];
        }
        else
        {
            moves = new Vector2[cost];

            for (int i = 0; i < cost - 1; i++)
            {
                moves[i] = previousMoves[i];
            }

            moves[cost - 1] = currentMove;
        }
    }

    /** Returns true if the coordinate (move) is part of the path. **/
    public bool ContainsMove(Vector2 move)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] == move)
            {
                return true;
            }
        }
        return false;
    }
}