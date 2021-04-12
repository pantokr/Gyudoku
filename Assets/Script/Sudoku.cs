using UnityEngine;

public class Sudoku
{
    private int[,] _solution;
    private int[,] _matrix;
    private int _size;
    private int _squareRootOfSize;
    private int _toRemove;

    public Sudoku(int toRemove)
    {
        _size = 9;
        _toRemove = toRemove;
        _matrix = new int[_size, _size];
        _solution = new int[_size, _size];
        _squareRootOfSize = 3;
    }

    public void FillValues()
    {
        FillDiagonal();

        FillRemaining(0, _squareRootOfSize);

        for (int i = 0; i < _size; i++)
            for (int x = 0; x < _size; x++)
                _solution[i, x] = _matrix[i, x];

        RemoveKDigits();
    }

    // Fill the diagonal SRN number of SRN x SRN matrices
    void FillDiagonal()
    {

        for (int i = 0; i < _size; i += _squareRootOfSize)

            // for diagonal box, start coordinates->i==j
            FillBox(i, i);
    }

    // Returns false if given 3 x 3 block contains num.
    bool UnUsedInBox(int rowStart, int colStart, int num)
    {
        for (int i = 0; i < _squareRootOfSize; i++)
            for (int j = 0; j < _squareRootOfSize; j++)
                if (_matrix[rowStart + i, colStart + j] == num)
                    return false;

        return true;
    }

    // Fill a 3 x 3 matrix.
    void FillBox(int row, int col)
    {
        int num;
        for (int i = 0; i < _squareRootOfSize; i++)
        {
            for (int j = 0; j < _squareRootOfSize; j++)
            {
                do
                { 
                    // 1부터 9랜덤 생성
                    num = RandomGenerator(_size);
                }
                while (!UnUsedInBox(row, col, num));

                _matrix[row + i, col + j] = num;
            }
        }
    }

    // Random generator
    static int RandomGenerator(int num)
    {
        return Random.Range(1, num + 1);
    }

    // Check if safe to put in cell
    bool CheckIfSafe(int i, int j, int num)
    {
        return (UnUsedInRow(i, num) &&
                UnUsedInCol(j, num) &&
                UnUsedInBox(i - i % _squareRootOfSize, j - j % _squareRootOfSize, num));
    }

    // check in the row for existence
    bool UnUsedInRow(int i, int num)
    {
        for (int j = 0; j < _size; j++)
            if (_matrix[i, j] == num)
                return false;
        return true;
    }

    // check in the row for existence
    bool UnUsedInCol(int j, int num)
    {
        for (int i = 0; i < _size; i++)
            if (_matrix[i, j] == num)
                return false;
        return true;
    }

    // A recursive function to fill remaining
    // matrix
    bool FillRemaining(int i, int j)
    {
        // System.out.println(i+" "+j);
        if (j >= _size && i < _size - 1)
        {
            i = i + 1;
            j = 0;
        }
        if (i >= _size && j >= _size)
            return true;

        if (i < _squareRootOfSize)
        {
            if (j < _squareRootOfSize)
                j = _squareRootOfSize;
        }
        else if (i < _size - _squareRootOfSize)
        {
            if (j == (int)(i / _squareRootOfSize) * _squareRootOfSize)
                j = j + _squareRootOfSize;
        }
        else
        {
            if (j == _size - _squareRootOfSize)
            {
                i = i + 1;
                j = 0;
                if (i >= _size)
                    return true;
            }
        }

        for (int num = 1; num <= _size; num++)
        {
            if (CheckIfSafe(i, j, num))
            {
                _matrix[i, j] = num;
                if (FillRemaining(i, j + 1))
                    return true;

                _matrix[i, j] = 0;
            }
        }
        return false;
    }

    // Remove the K no. of digits to
    // complete game
    void RemoveKDigits()
    {
        int count = _toRemove;
        while (count != 0)
        {
            int cellId = RandomGenerator(_size * _size);

            int i = (cellId / _size) % 9;
            int j = cellId % 9;
            if (j != 0)
                j = j - 1;

            if (_matrix[i, j] != 0)
            {
                count--;
                _matrix[i, j] = 0;
            }
        }
    }

    public int GETValue(int x, int y)
    {
        return _matrix[x, y];
    }

    public int GETAnswer(int x, int y)
    {
        return _solution[x, y];
    }

    public void SetValue(int x, int y, int value)
    {
        _matrix[x, y] = value;
    }

    public bool IsCorrect(int x, int y)
    {
        return (_solution[x, y] == _matrix[x, y]);
    }

    public bool Completed()
    {
        for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
                if (_matrix[x, y] != _solution[x, y])
                    return false;

        return true;
    }
}
