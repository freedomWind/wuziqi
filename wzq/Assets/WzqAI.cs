using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 五子棋AI
/// </summary>
public class WzqAI
{
    static WzqAI _ins;
    public static WzqAI GetIns()
    {
        if (_ins == null)
            _ins = new WzqAI();
        return _ins;
    }
    public void SetGame(WZQGame g)
    {
        wzq = g;
    }
    WZQGame wzq;
    /// <summary>
    /// 1,获取落子点的可赢矩阵
    /// 2，获取可赢矩阵中有效落子点
    /// 3，计算有效落子点的可赢权值
    /// 4，排序
    /// 5，取最大权值落子
    /// </summary>

    //分析有效落子点的可赢权值
    /*
     1, 试探性落子
     2，计算这个落子的可赢矩阵
     3，计算矩阵每个序列上的连续值情况
     4，统计=5个数；统计=4个数；统计=3个数；统计=2个数
     5，如果存在统计=5或者=4
         */
    public int t(Pos p)
    {
        #region  //计算有效落子点
        int[][] matraix = wzq.GetWinMatraix(p);
        List<int> plist = new List<int>();                
        for (int i = 0; i < matraix.GetLength(0); i++)
        {
            for (int j = 0; j < matraix[i].Length; j++)
            {
                byte r = 3;
                wzq.indexOf(matraix[i][j],ref r);
                if (r == 0)
                    plist.Add(matraix[i][j]);
            }
        }
        #endregion

        #region 计算有效落子点的所有矩阵
        Dictionary<int, int[][]> totalMatriaxDic = new Dictionary<int, int[][]>();
        plist.ForEach(_=> {
            wzq.XiaQi(_,false);    //试探性落子矩阵
            int[][] tempM = wzq.GetWinMatraix(Wuziqi.indexOf(_)); //获取该点的可赢
            totalMatriaxDic.Add(_, tempM);
        });
        #endregion
        #region  
        byte b = 3;
        wzq.indexOf(Wuziqi.indexOf(p.x,p.y), ref b);
        List<KeyValuePair<int, int>> totalPointValueList = new List<KeyValuePair<int, int>>();
        foreach (KeyValuePair<int, int[][]> kp in totalMatriaxDic)
        {
            if (kp.Key == -1) continue;
            totalPointValueList.Add(new KeyValuePair<int, int>(kp.Key,caculateQ(matraixValue(kp.Value, 1, 5,b))));
            wzq.BackQi(kp.Key);  //恢复棋子
        }
        #endregion
        #region  排序权值
        totalPointValueList.Sort((x,y)=> {
            if (x.Value < y.Value)
                return 1;
            else if (x.Value > y.Value)
                return -1;
            return 0;
        });
        return totalPointValueList[0].Key;
        #endregion
    }

    int[] matraixValue(int[][] m,int min,int max,byte b)
    {   
        int[] arry = new int[max - min + 1];
        int start = 0;
        for (int k = max; k >= min; k--)
        {
            int seq = 0;
            for (int i = 0; i < m.GetLength(0); i++)
            {
                if (wzq.isNSequence(m[i], k,b))
                    seq++;
            }
            arry[start++] = seq;
        }
        return arry;
    }

    public static int caculateQ(int[] ar)
    {
        int k = ar.Length-1;
        int v = 0;
        for (int i = 0; i < ar.Length; i++)
        {
            v += ar[i] * (int)Mathf.Pow(10, k);
            k--;
        }
        Debug.Log("qqq :"+v);
        return v;
    }
}

