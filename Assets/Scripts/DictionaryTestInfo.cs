using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <Summary>
/// 辞書のメソッドごとの処理時間に関する情報を格納するクラス
/// </Summary>
public class DictionaryTestInfo
{
    /// <Summary>
    /// 平均を計算後の処理時間
    /// </Summary>
    double _averageTime;

    /// <Summary>
    /// 平均を計算後の処理時間
    /// </Summary>
    public double AverageTime
    {
        get {return _averageTime;}
        private set {_averageTime = value;}
    }

    /// <Summary>
    /// 処理時間の最大値
    /// </Summary>
    double _maxTime = double.MinValue;

    /// <Summary>
    /// 処理時間の最大値
    /// </Summary>
    public double MaxTime
    {
        get {return _maxTime;}
        private set {_maxTime = value;}
    }

    /// <Summary>
    /// 処理時間の最小値
    /// </Summary>
    double _minTime = double.MaxValue;

    /// <Summary>
    /// 処理時間の最小値
    /// </Summary>
    public double MinTime
    {
        get {return _minTime;}
        private set {_minTime = value;}
    }

    /// <Summary>
    /// 既存の値より処理時間が大きくなる場合に最大値をセットする
    /// </Summary>
    void CheckMaxTime(double value)
    {
        if (value > _maxTime)
        {
            _maxTime = value;
        }
    }

    /// <Summary>
    /// 既存の値より処理時間が小さくなる場合に最小値をセットする
    /// </Summary>
    void CheckMinTime(double value)
    {
        if (value < _minTime)
        {
            _minTime = value;
        }
    }

    /// <Summary>
    /// 既存の値より処理時間が大きくなる場合に最大値をセットする
    /// </Summary>
    public void CheckTime(double value)
    {
        CheckMaxTime(value);
        CheckMinTime(value);
    }

    /// <Summary>
    /// 平均処理時間をセットする
    /// </Summary>
    public void SetAverageTime(double value)
    {
        _averageTime = value;
    }
}
