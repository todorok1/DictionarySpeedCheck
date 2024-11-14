using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <Summary>
/// 辞書のメソッドをテストした際の出力対象情報を格納するクラス
/// </Summary>
public class DictionaryTestOutputInfo
{
    /// <Summary>
    /// 実行したメソッド名
    /// </Summary>
    public string _methodName;

    /// <Summary>
    /// データ数
    /// </Summary>
    public int _dataSize;

    /// <Summary>
    /// 平均をとる試行回数
    /// </Summary>
    public int _average;

    /// <Summary>
    /// 平均処理時間
    /// </Summary>
    public double _processTime;

    /// <Summary>
    /// 最大の処理時間
    /// </Summary>
    public double _processTimeMax;

    /// <Summary>
    /// 最小の処理時間
    /// </Summary>
    public double _processTimeMin;

    public DictionaryTestOutputInfo(string methodName, int dataSize, int average, double processTime, double processTimeMax, double processTimeMin)
    {
        _methodName = methodName;
        _dataSize = dataSize;
        _average = average;
        _processTime = processTime;
        _processTimeMax = processTimeMax;
        _processTimeMin = processTimeMin;
    }
}
