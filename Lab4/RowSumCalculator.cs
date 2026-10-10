namespace Lab4;

public delegate int[] RowSumsOperation(int[,] matrix);

public static class RowSumCalculator
{
    /// <summary>Обчислює суму елементів кожного рядка матриці.</summary>
    public static int[] Calculate(int[,] matrix)
    {
        int[] sums = new int[matrix.GetLength(0)];

        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int column = 0; column < matrix.GetLength(1); column++)
            {
                sums[row] += matrix[row, column];
            }
        }

        return sums;
    }
}
