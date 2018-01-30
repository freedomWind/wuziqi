using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WZQ_GUI : MonoBehaviour
{
    public Text tip;
    private WZQGame wzq;
    public Transform qiTrans;
    public GameObject whiteQ;
    public GameObject blackQ;
    public Canvas canvas;
    Vector2 final = new Vector2(237.5f, 243);
    Vector2 zero = new Vector2(-237.5f, -231);

    Wuziqi.qiType myturn;

    void delayaXiaqi(int idx)
    {
        StartCoroutine(delayXiaqi(idx));
    }
    IEnumerator delayXiaqi(int idx)
    {
        int z = WzqAI.GetIns().t(Wuziqi.indexOf(idx));
        yield return new WaitForSeconds(1.5f);
        PrintQizi(z);
    }
    private void Start()
    {
        wzq = new WZQGame();
        wzq.gStateChangeDe = onGameStateChange;
        wzq.gResultDel = onGameResult;
        wzq.gameStart(Wuziqi.qiType.white);
        myturn = Wuziqi.qiType.white;
        WzqAI.GetIns().SetGame(wzq);

        Debug.Log("3215="+ WzqAI.caculateQ(new int[] { 3, 2, 1, 5 }));
    }
    int myPosIndex;
    private void Update()
    {
        if (wzq.curState != WZQGame.gState.gaming) return;
        if (wzq.curTurn == myturn)
        {
            if (Input.GetMouseButtonUp(0))
            {
                myPosIndex = PrintQizi();
            }
            //AI
            if (wzq.curTurn != myturn)
            {
                delayaXiaqi(myPosIndex);
            }
        }
    }
    void onGameStateChange(WZQGame.gState state)
    {
        if (state == WZQGame.gState.gaming)
            ShowTip("游戏开始");
        else if (state == WZQGame.gState.ready)
            ShowTip("游戏准备就绪");
        else if (state == WZQGame.gState.over)
            ShowTip("游戏结束");
    }
    void onGameResult(Wuziqi.gResult result)
    {
        if (result == Wuziqi.gResult.blackWin)
            ShowTip("黑棋赢了");
        else if (result == Wuziqi.gResult.whiteWin)
            ShowTip("白棋赢了");
        else if (result == Wuziqi.gResult.noWin)
            ShowTip("和局");
    }
    void PrintQizi(int idx)
    {
        if (idx == -1)
        {
            ShowTip("无效落子"); return;
        }
        GameObject oo = null;
        string f = "黑棋";
        if (wzq.curTurn == Wuziqi.qiType.white)
        {
            f = "白棋";
            oo = GameObject.Instantiate(whiteQ) as GameObject;
        }
        else
            oo = GameObject.Instantiate(blackQ) as GameObject;
        if (wzq.XiaQi(idx))
        {
            ShowTip(f + "落子成功");
            Vector2 p = getDropPointByIndex(idx);
            oo.transform.SetParent(qiTrans);
            oo.transform.localScale = Vector3.one;
            oo.transform.localPosition = p;
        }
        else
        {
            if (wzq.curState != WZQGame.gState.over)
                ShowTip("无效落子");
        }
    }
    //void On
    int PrintQizi()
    {
        int idx = getIndexByClick();
        PrintQizi(idx);
        return idx;
    }
    void ShowTip(string str)
    {
        tip.text = str;
    }
    int getIndexByClick()
    {
        Vector2 dPos = Vector2.one;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        Input.mousePosition, canvas.worldCamera, out dPos);
        if (dPos.x < zero.x || dPos.x > final.x || dPos.y < zero.y || dPos.y > final.y)
            return -1;
        int x = (int)((dPos.x - zero.x) / 60);
        if ((int)(dPos.x - zero.x) % 60 > 30) x++;
        int y = (int)(dPos.y - zero.y) / 60;
        if ((int)(dPos.y - zero.y) % 60 > 30) y++;
        return Wuziqi.indexOf(x,y);
    }
    Vector2 getDropPointByIndex(Pos p)
    {
        return zero + new Vector2(p.x * 60, p.y * 60);
    }
    Vector2 getDropPointByIndex(int idx)
    {
        Pos p = Wuziqi.indexOf(idx);
        return zero + new Vector2(p.x * 60, p.y * 60);
    }
}