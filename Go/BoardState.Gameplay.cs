﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Go
{
    partial struct BoardState
    {
        public Player Winner
        {
            get
            {
                if (!Terminal)
                {
                    throw new InvalidOperationException("Game is not finished");
                }
                //TODO: count territory belonging to both players
                List<StoneGroup> black = StoneGroups(Stone.Black);
                List<StoneGroup> white = StoneGroups(Stone.White);
                List<StoneGroup> empty = StoneGroups(Stone.Empty);
                int blackTerritory = black.Sum(g => g.Intersections.Count);
                int whiteTerritory = white.Sum(g => g.Intersections.Count);
                BoardState @this = this; //Copy so the lambda can capture
                foreach (StoneGroup group in empty)
                {
                    bool touchesBlack = group.Intersections.SelectMany(i => @this.Adjacencies(i)).Any(i => @this[i] == Stone.Black);
                    bool touchesWhite = group.Intersections.SelectMany(i => @this.Adjacencies(i)).Any(i => @this[i] == Stone.White);
                    if (touchesWhite && !touchesBlack)
                    {
                        whiteTerritory += group.Intersections.Count;
                    }
                    else if (touchesBlack && !touchesWhite)
                    {
                        blackTerritory += group.Intersections.Count;
                    }
                }

                if (blackTerritory > whiteTerritory)
                {
                    return Player.Black;
                }
                else if (whiteTerritory > blackTerritory)
                {
                    return Player.White;
                }
                else
                {
                    return Player.None;
                }
            }
        }

        public BoardState? MakePlay(Play play)
        {
            if (Terminal) //Prevent plays after finishing the game
            {
                return null;
            }
            if (play is PassPlay)
            {
                Player nextPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
                //Two passes in a row ends the game
                bool terminal = WasPass;
                return new BoardState(new BoardStateNode(this), state.DeepCopy(), nextPlayer, wasPass: true, terminal);
            }
            else if (play is MovePlay move)
            {
                return MakeMove(move);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private BoardState? MakeMove(MovePlay move)
        {
            Point intersection = move.Intersection;
            if (!Contains(intersection) || this[intersection] != Stone.Empty)
            {
                return null;
            }

            Stone[,] newState = state.DeepCopy();
            newState[intersection.Y, intersection.X] = (Stone)CurrentPlayer;
            Player nextPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
            BoardState newBoardState = new BoardState(new BoardStateNode(this), newState, nextPlayer);

            //Remove stones of the opponent's color that do not have liberties (they are surrounded)
            foreach (StoneGroup group in newBoardState.StoneGroups((Stone)nextPlayer))
            {
                //Only have to check one of the intersections because
                //connected stones either all have liberties or all don't
                if (!newBoardState.HasLiberties(group.Intersections[0]))
                {
                    foreach (Point i in group.Intersections)
                    {
                        newState[i.Y, i.X] = Stone.Empty;
                    }
                }
            }

            //Self-capture (removing liberties from one's own pieces) is prohibited
            //This can only be checked after the opponent's stones have been removed.
            foreach (StoneGroup group in newBoardState.StoneGroups((Stone)CurrentPlayer))
            {
                if (!newBoardState.HasLiberties(group.Intersections[0]))
                {
                    return null;
                }
            }

            for (BoardStateNode node = newBoardState.Previous; node != null; node = node.Previous)
            {
                //repetition of a previous state is not allowed
                if (node.State.Equals(newState))
                {
                    return null;
                }
            }
            return newBoardState;
        }

        public List<StoneGroup> StoneGroups(params Stone[] wanted)
        {
            BoardState @this = this; //Make a copy because DFS can't capture this
            bool[,] visited = new bool[state.GetLength(0), state.GetLength(1)];

            void DFS(Point intersection, StoneGroup group)
            {
                if (!visited[intersection.Y, intersection.X] && @this[intersection] == group.Stone)
                {
                    visited[intersection.Y, intersection.X] = true;
                    group.Intersections.Add(intersection);
                    foreach (Point adj in @this.Adjacencies(intersection))
                    {
                        DFS(adj, group);
                    }
                }
            }

            List<StoneGroup> groups = new List<StoneGroup>();
            for (int row = 0; row < state.GetLength(0); row++)
            {
                for (int col = 0; col < state.GetLength(1); col++)
                {
                    Point intersection = new Point(col, row);
                    if (wanted.Contains(this[intersection]))
                    {
                        StoneGroup group = new StoneGroup(this[intersection], new List<Point>());
                        DFS(intersection, group);
                        if (group.Intersections.Any())
                        {
                            groups.Add(group);
                        }
                    }
                }
            }
            return groups;
        }

        //A stone has a liberty if it is:
        // -adjacent to an empty intersection
        // -or connected to a stone with a liberty
        public bool HasLiberties(Point intersection)
        {
            Stone stone = this[intersection];
            if (stone == Stone.Empty)
            {
                return true;
            }

            BoardState @this = this; //Make a copy because DFS can't capture this
            bool[,] visited = new bool[state.GetLength(0), state.GetLength(1)];
            bool DFS(Point intersection)
            {
                if (visited[intersection.Y, intersection.X])
                {
                    return false;
                }
                if (@this.Adjacencies(intersection).Any(i => @this[i] == Stone.Empty))
                {
                    return true;
                }
                visited[intersection.Y, intersection.X] = true;
                return @this.Adjacencies(intersection).Where(i => @this[i] == stone).Any(DFS);
            }

            return DFS(intersection);
        }

        private IEnumerable<Point> Adjacencies(Point intersection)
        {
            return new List<Point>() {
            new Point(intersection.X - 1, intersection.Y),
            new Point(intersection.X + 1, intersection.Y),
            new Point(intersection.X, intersection.Y - 1),
            new Point(intersection.X, intersection.Y + 1)
            }.Where(Contains).ToList();
        }
    }
}
