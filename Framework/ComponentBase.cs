
using UnityEngine;
using System;
using System.Collections;
using Sirenix.OdinInspector;

public class ComponentBase : SerializedMonoBehaviour
{
    #region PRIVATE

    private IEnumerator CoroutineCondition(Func<bool> conditionToExecute, Func<bool> conditionBreak, Action func)
    {
        if (conditionBreak()) yield break;

        while (!conditionToExecute())
        {
            if (conditionBreak()) yield break;

            yield return new WaitForSeconds(0.0f);
        }

        func();
    }

    private IEnumerator CoroutineTimerCondition(float time, Func<bool> conditionBreak, Action func)
    {
        if (conditionBreak()) yield break;

        float startTime = Time.time;

        while (startTime + time < Time.time)
        {
            if (conditionBreak()) yield break;

            yield return new WaitForSeconds(0.0f);
        }

        func();
    }

    private IEnumerator CoroutineTimer(float time, Action func)
    {
        yield return new WaitForSeconds(time);

        func();
    }

    private IEnumerator CoroutineEndOfFrame(Action func)
    {
        yield return new WaitForEndOfFrame();

        func();
    }

    private IEnumerator CoroutineNextFrame(Action func)
    {
        yield return null;

        //		yield return new WaitForEndOfFrame();
        //		yield return new WaitForEndOfFrame();

        func();
    }

    #endregion PRIVATE

    /// <summary>
    /// conditionToExecute 함수가 true를 반환하면 func 함수를 실행합니다.
    /// </summary>
    protected void StartCoroutineCondition(Func<bool> condition, Action func)
    {
        StartCoroutine(CoroutineCondition(condition, delegate () { return false; }, func));
    }

    /// <summary>
    /// conditionToExecute 함수가 true를 반환하면 func 함수를 실행합니다만, conditionBreak 함수가 true를 반환하면 코루틴을 중지합니다.
    /// conditionBreak는 최초 한번, conditionToExecute가 true가 되도록 기다리는 루프의 매회 체크합니다.
    /// </summary>
    protected void StartCoroutineCondition(Func<bool> conditionToExecute, Func<bool> conditionBreak, Action func)
    {
        StartCoroutine(CoroutineCondition(conditionToExecute, conditionBreak, func));
    }

    /// <summary>
    /// time(단위 : 초)에 지정된 시간이 지나면 func 함수를 실행합니다.
    /// </summary>
    protected void StartCoroutineTimer(float time, Action func)
    {
        StartCoroutine(CoroutineTimer(time, func));
    }

    /// <summary>
    /// time(단위 : 초)에 지정된 시간이 지나면 func 함수를 실행합니다만, 도중에 conditionBreak 함수가 true를 반환하면 코루틴을 중지합니다.
    /// </summary>
    protected void StartCoroutineTimerCondition(float time, Func<bool> condition, Action func)
    {
        StartCoroutine(CoroutineTimerCondition(time, condition, func));
    }

    /// <summary>
    /// 모든 렌더링이 종료된 다음에 func 함수를 실행합니다.
    /// </summary>
    protected void StartCoroutineEndOfFrame(Action func)
    {
        StartCoroutine(CoroutineEndOfFrame(func));
    }

    /// <summary>
    /// 다음 프레임에서 func 함수를 호출합니다.
    /// </summary>
    protected void StartCoroutineNextFrame(Action func)
    {
        StartCoroutine(CoroutineNextFrame(func));
    }

    /// <summary>
    /// 몇 초뒤에 넘어갈지.
    /// </summary>
    /// <param name="checkTime"></param>
    /// <returns></returns>
    protected IEnumerator cCheckTime(float checkTime)
    {
        float time = checkTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
