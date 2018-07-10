using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 点
/// </summary>
public struct Pos
{
    public int x;
    public int y;
    public Pos(int x, int y)
    {
		
        this.x = x; this.y = y;
    }
    public bool Equals(Pos p)
    {
        return p.x == x && p.y == y;
    }
}
/// <summary>
/// 棋盘点
/// </summary>
public struct qPos
{
    public qPos(int x,int y)
    {
        pos = new Pos(x, y); r = 0;
    }
    public Pos pos;   //位置     
    public byte r;    //黑子或白子或空

}
public class WZQGame : Wuziqi
{
    public enum gState
    {
        ready,
        gaming,
        over,
    }
    public System.Action<Wuziqi.gResult> gResultDel;
    public System.Action<gState> gStateChangeDe;
    Wuziqi.qiType firstTurn;
    Wuziqi.qiType _curTurn;
    gState _curState = gState.ready;
    public Wuziqi.qiType curTurn
    {
        get { return _curTurn; }
    }
    public gState curState
    {
        get { return _curState; }
        private set {
            if (_curState != value)
                gStateChangeDe?.Invoke(value);
            _curState = value;
        }
    }
    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="firstTurn"></param>
    public void gameStart(Wuziqi.qiType firstTurn)
    {
        this.firstTurn = firstTurn;
        this.curState = gState.gaming;
        this._curTurn = firstTurn;        
    }
    public bool XiaQi(int index,bool flag = true)
    {
        return XiaQi(Wuziqi.indexOf(index),flag);
    }
    public bool XiaQi(Pos p,bool flag = true)
    {
        if (curState != gState.gaming) return false;
        if (AddQi(p, curTurn,flag))
        {
            if (flag)
            {
                if (_curTurn == qiType.white && whiteCount >= 5) //判断
                {
                    if (isWin(p)) gResultDel?.Invoke(gResult.whiteWin);
                }
                if (curTurn == qiType.black && blackCount >= 5)
                {
                    if (isWin(p)) gResultDel?.Invoke(gResult.blackWin);
                }
                _curTurn = qiType.white == _curTurn ? qiType.black : qiType.white;
                if (whiteCount + blackCount > 81)
                    gResultDel?.Invoke(gResult.noWin);
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 裁剪可赢矩阵
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public int[][] GetWinMatraix(Pos p)
    {
        if (Wuziqi.indexOf(p.x, p.y) == -1) return null;
        int[][] win = getWinPos(p);
        byte q = 4;
        indexOf(indexOf(p.x,p.y), ref q);
        for (int i = 0; i < win.GetLength(0); i++)
        {
            for (int j = 0; j < win[i].Length; j++)    
            {
                if (win[i][j] == -1) continue;
                byte b = 3;
                indexOf(win[i][j], ref b);
                if (b == 0) continue;
                if (b != q)
                    win[i][j] = -1;
            }
        }
        return win;
    }
}
public class Wuziqi
{
    /// <summary>
    /// 棋子类型
    /// </summary>

    public enum qiType
    {
        black,
        white,
    }
    /// <summary>
    /// 游戏结果
    /// </summary>
    public enum gResult
    {
        none,
        blackWin,
        whiteWin,
        noWin,     //平局
    }
    qPos[] qipan;   //棋盘
    int _whiteCount;
    int _blackCount;
    public int whiteCount
    {
        get
        {
            return _whiteCount;
        }
    }
    public int blackCount
    {
        get
        {
            return _blackCount;
        }
    }

    public Wuziqi()
    {
        Init();
    }
    protected void Init()
    {
        if (qipan == null)
        {
            qipan = new qPos[81];
            for (int i = 0; i < 81; i++)
            {
                int column = i % 9;
                int row = i / 9;
                qPos p = new qPos(column, row);
            }
        }
        for (int i = 0; i < qipan.Length; i++)
            qipan[i].r = 0;
        _whiteCount = _blackCount = 0;
    }
    /// <summary>
    /// 下棋
    /// </summary>
    /// <param name="p"></param>
    /// <param name="type"></param>
    /// <param name="flag">flag = false表示模拟</param>
    /// <returns></returns>
    protected bool AddQi(Pos p, qiType type, bool flag = true)
    {
        int index = indexOf(p.x, p.y);
        if (index == -1 || qipan[index].r != 0) return false;
        if (type == qiType.black)
        {
            qipan[index].r = 1; if(flag) _blackCount++;
        }
        else
        {
            qipan[index].r = 2; if(flag) _whiteCount++;
        }
        return true;
    }
    public void BackQi(Pos p)
    { }
    public void BackQi(int index)
    {
        if (index < 0 || index > 81)
            return;
        qipan[index].r = 0;
    }
    /// <summary>
    /// 棋盘任意位置的索引值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int indexOf(int x, int y)
    {
        if (x < 0 || x > 8 || y < 0 || y > 8)
            return -1;
        return y * 9 + x;
    }
    public static Pos indexOf(int index)
    {
        if (index < 0 || index > 81)
            return new Pos(-1, -1);
        return new Pos(index % 9, index / 9);
    }
    public void indexOf(int index,ref byte r)
    {
        if (index < 0 || index > 81)
            return;
        r = qipan[index].r;
    }
    /// <summary>
    /// 判断某个点是否赢
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool isWin(Pos p)
    {       
        int[][] winp = getWinPos(p);
        for (int i = 0; i < winp.GetLength(0); i++)
        {
            if (isNSequence(winp[i], 5)) return true;
            else continue;
        }
        return false;
    }
    public bool isNSequence(int[] array, int n, byte b)
    {
        if (array.Length < n)
            return false;
        int p, q = 1;
        int N = 1;
        for (int i = 0; i < array.Length - 1; i++)
        {
            p = array[i]; q = array[i + 1];
            //遇到未知点或空位或者棋子不连续
            if (p == -1 || q == -1 || qipan[p].r == 0 || qipan[p].r != qipan[q].r ||qipan[p].r != b)//|| !qipan[p].Equals(qipan[q]))
            {
                N = 1; continue;
            }
            N++;
            if (N == 5) return true;
        }
        return false;
    }
    /// <summary>
    /// 判断一个连续的序列中是否有n个连续相同的
    /// </summary>
    /// <param name="array"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public bool isNSequence(int[] array,int n)
    {
        if (array.Length < n)
            return false;
        int p,q = 1;
        int N = 1;
        for (int i = 0; i < array.Length -1; i++)
        {
            p = array[i]; q = array[i + 1];
            //遇到未知点或空位或者棋子不连续
            if (p == -1 || q == -1 || qipan[p].r == 0 || qipan[p].r != qipan[q].r)//|| !qipan[p].Equals(qipan[q]))
            {
                N = 1; continue;
            }
            N++;
            if (N == 5) return true;
        }
        return false;
    }
    /// <summary>
    /// 计算任意棋子对应的赢的位置矩阵
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    protected int[][] getWinPos(Pos p)
    {
        List<Pos[]> plist = new List<Pos[]>();
        int[] p1 = new int[9];  //横
        int[] p2 = new int[9];  //竖
        int[] p3 = new int[9];  //左
        int[] p4 = new int[9];  //右
        for (int j = 0; j < 9; j++)
        {
            p1[j] = indexOf(p.x - 4 + j, p.y);           
            p2[j] = indexOf(p.x, p.y - 4 + j);
            p3[j] = indexOf(p.x - 4 + j, p.y + 4 - j);
            p4[j] = indexOf(p.x - 4 + j, p.y - 4 + j);
        }
        return new int[][] {p1,p2,p3,p4};
    }
}
