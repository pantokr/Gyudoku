using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingNumberMaker
{
    private int[,] dst;

    public MissingNumberMaker(int[,] dst)
    {
        this.dst = dst;
    }

    public int GetValue(int y, int x)
    {
        return dst[y, x];
    }

    #region sudoku missing number algorithm

    public void EmptySubGrid(int emptyC1, int emptyC2, int emptyMiddle, int patternCode = 0)
    {
        if (patternCode == 0)
        {
            //0,2,6,8
            int[,] shapeSubGrid = CreateSubGridShape(emptyC1);
            CombineSubGrid(0, MirrorSubGrid(shapeSubGrid, 0));
            CombineSubGrid(2, MirrorSubGrid(shapeSubGrid, 1));
            CombineSubGrid(6, MirrorSubGrid(shapeSubGrid, 2));
            CombineSubGrid(8, MirrorSubGrid(shapeSubGrid, 3));

            //1,3,5,7
            shapeSubGrid = CreateSubGridShape(emptyC2);
            CombineSubGrid(1, RotateSubGrid(shapeSubGrid, 0));
            CombineSubGrid(3, RotateSubGrid(shapeSubGrid, 1));
            CombineSubGrid(5, RotateSubGrid(shapeSubGrid, 3));
            CombineSubGrid(7, RotateSubGrid(shapeSubGrid, 2));

            //4
            shapeSubGrid = CreateSubGridShape(emptyMiddle, 1);
            CombineSubGrid(4, shapeSubGrid);
        }
        else
        {
            //0,2,6,8
            int[,] shapeSubGrid = CreateSubGridShape(emptyC1);
            CombineSubGrid(0, MirrorSubGrid(shapeSubGrid, 0));
            CombineSubGrid(8, MirrorSubGrid(shapeSubGrid, 3));
            
            shapeSubGrid = CreateSubGridShape(emptyC1);
            CombineSubGrid(2, MirrorSubGrid(shapeSubGrid, 0));
            CombineSubGrid(6, MirrorSubGrid(shapeSubGrid, 3));

            //1,3,5,7
            shapeSubGrid = CreateSubGridShape(emptyC2);
            CombineSubGrid(1, MirrorSubGrid(shapeSubGrid, 0));
            CombineSubGrid(7, MirrorSubGrid(shapeSubGrid, 3));
            
            shapeSubGrid = CreateSubGridShape(emptyC2);
            CombineSubGrid(3, MirrorSubGrid(shapeSubGrid, 0));
            CombineSubGrid(5, MirrorSubGrid(shapeSubGrid, 1));

            //4
            shapeSubGrid = CreateSubGridShape(emptyMiddle, 1);
            CombineSubGrid(4, shapeSubGrid);
        }
    }

    //����׸��� ���� ��ȯ �Լ�
    private int[,] CreateSubGridShape(int emptyCnt, int isMiddle = 0)
    {
        if (isMiddle == 0)
        {
            int[] asc = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            int[,] shapeSubGrid = new int[3, 3];
            //���� ����Ʈ ����
            for (int index = 0; index < 9; index++)
            {
                int rand = Random.Range(0, 9);

                int t = asc[rand];
                asc[rand] = asc[index];
                asc[index] = t;
            }

            for (int yx = 0; yx < emptyCnt; yx++)
            {
                int emptyCell = asc[yx];
                int tY = emptyCell / 3;
                int tX = emptyCell % 3;
                shapeSubGrid[tY, tX] = 1;
            }

            return shapeSubGrid;
        }
        else
        {
            int[] asc = { 0, 1, 2, 3, 4 };
            int[,] shapeSubGrid = new int[3, 3];

            //���� ����Ʈ ����
            for (int index = 0; index < 5; index++)
            {
                int rand = Random.Range(0, 5);

                int t = asc[rand];
                asc[rand] = asc[index];
                asc[index] = t;
            }

            for (int yx = 0; yx < emptyCnt; yx++)
            {
                int emptyCell = asc[yx];

                int tY = emptyCell / 3;
                int tX = emptyCell % 3;
                shapeSubGrid[tY, tX] = 1;
                shapeSubGrid[2 - tY, 2 - tX] = 1;
            }

            PrintSubGrid(shapeSubGrid);

            return shapeSubGrid;
        }
    }
    
    #endregion

    //����׸��� ���� �迭�� dst�� ����׸��� �� �ϳ��� ������
    private void CombineSubGrid(int subGridIndex, int[,] subGrid)
    {
        int sY = subGridIndex / 3;
        int sX = subGridIndex % 3;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (subGrid[y, x] == 1)
                {
                    dst[sY * 3 + y, sX * 3 + x] = 0;
                }
            }
        }
    }

    //����׸��� ��Ī�� ��� �� ��ȯ �Լ�
    private int[,] MirrorSubGrid(int[,] subGrid, int mirroringCode = 0)
    {
        if (mirroringCode == 0) // �״�� ��ȯ
        {
            return subGrid;
        }
        else if (mirroringCode == 1) // �¿� ��Ī
        {
            int[,] tSubGrid = new int[3, 3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    tSubGrid[y, x] = subGrid[y, 2 - x];
                }
            }
            return tSubGrid;
        }
        else if (mirroringCode == 2)// ���� ��Ī
        {
            int[,] tSubGrid = new int[3, 3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    tSubGrid[y, x] = subGrid[2 - y, x];
                }
            }
            return tSubGrid;
        }
        else // �� ��Ī
        {
            int[,] tSubGrid = new int[3, 3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    tSubGrid[y, x] = subGrid[2 - y, 2 - x];
                }
            }
            return tSubGrid;
        }
    }

    //����׸��� ȸ���� ��� �� ��ȯ �Լ�
    private int[,] RotateSubGrid(int[,] subGrid, int mirroringCode = 0)
    {
        if (mirroringCode == 0) // �״�� ��ȯ
        {
            return subGrid;
        }
        else if (mirroringCode == 1) // 270�� ȸ��
        {
            int[,] tSubGrid = new int[3, 3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    tSubGrid[y, x] = subGrid[2 - x, y];
                }
            }
            return tSubGrid;
        }
        else if (mirroringCode == 2)// 180�� ȸ��
        {
            return MirrorSubGrid(subGrid, 3);
        }

        else // ������ 90�� ȸ��
        {
            int[,] tSubGrid = new int[3, 3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    tSubGrid[y, x] = subGrid[x, 2 - y];
                }
            }
            return tSubGrid;
        }
    }

    private void PrintSubGrid(int[,] subGrid)
    {
        for (int y = 0; y < 3; y++)
        {
            string tString = "";
            for (int x = 0; x < 3; x++)
            {
                tString += $"{subGrid[y, x]}";
            }
        }
    }
}
