using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotManager : MonoBehaviour
{
    public GameObject[] reels;
    public float spinSpeed = 1000.0f;
    public float stopDelay = 0.5f;
    public int localResultIndex = 0;    //(仮)最終的にコメントアウト予定
    bool[] isPersonSpinning = { false, false, false };
    public OptionData optionData;

    void Update()
    {
        if (GameManager.isSpinning == true)
        {
            for (int i = 0; i < reels.Length; i++)
            {
                if (isPersonSpinning[i] == true)
                {
                    reels[i].transform.Rotate(-spinSpeed * Time.deltaTime, 0, 0);
                }
            }
        }
    }

    public void StartSlot()
    {
        if (GameManager.isSpinning == false)
        {
            GameManager.result = null;
            GameManager.isSpinning = true;

            for (int i = 0; i < isPersonSpinning.Length; i++)
            {
                isPersonSpinning[i] = true;
            }

            StartCoroutine(StopReelsSequentially());
        }
    }

    //StartSlotメソッドまで作成できたらコメントアウトを解除
    IEnumerator StopReelsSequentially()
    {
        yield return new WaitForSeconds(3.0f);
        //yield return new WaitUntil(() => GameManager.isResult);

        for (int i = 0; i < reels.Length; i++)
        {
            //float targetAngle = GameManager.resultIndex * (360f / optionData.option.prizeName.Length + 1); // 対応する角度を計算
            float targetAngle = localResultIndex * (360f / optionData.option.prizeName.Length + 1); //仮値
            targetAngle -= 360.0f;
            isPersonSpinning[i] = false;

            yield return StartCoroutine(SlowStop(reels[i], targetAngle));
        }

        // 回転フラグをオフ
        GameManager.isSpinning = false;

        GameManager.isLottery = false;

        // resultに結果を格納
        //GameManager.result = optionData.option.prizeName[GameManager.resultIndex];
        GameManager.result = optionData.option.prizeName[localResultIndex]; //仮値

    }

    //StartSlotメソッドまで作成できたらコメントアウトを解除
    IEnumerator SlowStop(GameObject reel, float targetAngle)
    {
        float currentAngle = reel.transform.rotation.eulerAngles.x;
        float difference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // ゆっくり止める
        while (Mathf.Abs(difference) > 0.1f)
        {
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 5f);
            reel.transform.rotation = Quaternion.Euler(currentAngle, 0, 0);
            difference = Mathf.DeltaAngle(currentAngle, targetAngle);
            yield return null;
        }

        // 最終位置をスナップ
        reel.transform.rotation = Quaternion.Euler(targetAngle, 0, 0);
    }
}
