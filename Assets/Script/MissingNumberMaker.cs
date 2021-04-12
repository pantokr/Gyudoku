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

    private void EmptySubGrid(int emptyC1, int emptyC2, int shapeCode = 0)
    {
        ////0,2,6,8
        //int[,] shapeSubGrid = CreateSubGridShape(emptyC1);
        //CombineSubGrid(0, shapeSubGrid, 0);
        ////1,3,5,7
        //int[,] shapeSubGrid = CreateSubGridShape(emptyC2);
        //int[,] tSubGrid = new int[3, 3];
        //int shapeSubGrid = CreateSubGridShape()
        ////서브그리드 복제
        //for (int y = 0; y < 3; y++)
        //{
        //    for (int x = 0; x < 3; x++)
        //    {
        //        tSubGrid[y, x] = dst[sY * 3 + y, sX * 3 + x];
        //    }
        //}


        ////서브그리드 적용
        //for (int y = 0; y < 3; y++)
        //{
        //    for (int x = 0; x < 3; x++)
        //    {
        //        dst[sY * 3 + y, sX * 3 + x] = tSubGrid[y, x];
        //    }
        //}
    }

    private int[,] CreateSubGridShape(int emptyCnt)
    {
        int[] asc = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        int[,] shapeSubGrid = { { 0 } };
        //랜덤 리스트 생성
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
    #endregion

    private void CombineSubGrid(int subGridCode, int[,] shapeSubGrid, int rotationCode)
    {

    }

    private int[,] MirrorSubGrid(int[,] subGrid, int mirroringCode = 0)
    {
        //if (mirroringCode == 0) // 그대로 반환
        //{
        //    return subGrid;
        //}
        //else if (mirroringCode == 1) // 좌우 대칭
        //{
        //    int[,] tSubGrid = new int[3, 3];
        //    tSubGrid[]
        //}
        //return subGrid;
    }
}
