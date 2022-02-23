using System;

namespace Go
{
    enum Stone
    {
        Empty,
        White,
        Black
    }

    static class StoneExtensions
    {
        public static bool Equals(this Stone[,] array, Stone[,] other)
        {
            if (other == null)
            {
                return false;
            }
            if (array.GetLength(0) != other.GetLength(0) || array.GetLength(1) != other.GetLength(1))
            {
                return false;
            }
            for (int row = array.GetLowerBound(0); row < array.GetUpperBound(0); row++)
            {
                for (int col = array.GetLowerBound(1); col < array.GetUpperBound(1); col++)
                {
                    if (array[row, col] != other[row, col])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static Stone[,] DeepCopy(this Stone[,] array)
        {
            Stone[,] copy = new Stone[array.GetLength(0), array.GetLength(1)];
            Array.Copy(array, copy, array.Length);
            return copy;
        }
    }
}
