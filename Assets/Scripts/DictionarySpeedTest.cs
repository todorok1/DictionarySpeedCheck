using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <Summary>
/// 辞書のメソッドごとの速度を比較するためのクラス
/// </Summary>
public class DictionarySpeedTest : MonoBehaviour
{
    /// <Summary>
    /// 処理を開始するフレーム数
    /// </Summary>
    [SerializeField]
    int _startFrame = 10;

    /// <Summary>
    /// 処理時間の平均を取るための試行回数
    /// </Summary>
    [SerializeField]
    int _averageCount = 10;

    /// <Summary>
    /// 事前に辞書のCapacityを指定するかどうかのフラグ
    /// Trueで指定する
    /// </Summary>
    bool _setCapacity = false;

    /// <Summary>
    /// テキストに出力する文字列を保持
    /// </Summary>
    List<string> _outputMessages = new();

    /// <Summary>
    /// 処理内容を渡すデリゲート
    /// </Summary>
    delegate double TestFunc(int processCount, bool isSetCapacity);

    /// <Summary>
    /// 値の追加時、重複を試みる1の位の数字
    /// </Summary>
    readonly int[] DuplicateCandidate = new int[]
    {
        0,
        5,
    };

    /// <Summary>
    /// 辞書サイズの配列
    /// </Summary>
    readonly int[] ProcessCountArray = new int[]
    {
        1000,
        10000,
        100000,
        1000000,
        10000000,
    };

    void Start()
    {
        
    }

    void Update()
    {
        ProcessKicker();
    }

    /// <Summary>
    /// 指定フレーム数後に処理を開始する
    /// ゲーム開始時はその他のクラスで処理が立て込んでしまうため
    /// </Summary>
    void ProcessKicker()
    {
        if (Time.frameCount != _startFrame)
        {
            return;
        }

        _setCapacity = true;
        ExecTest(GetTryAddProcessTime);
        ExecTest(GetTryAddProcessTimeIncludesDuplication);
        ExecTest(GetContainsKeyProcessTime);
        ExecTest(GetContainsKeyProcessTimeIncludesDuplication);

        _setCapacity = false;
        ExecTest(GetTryAddProcessTime);
        ExecTest(GetTryAddProcessTimeIncludesDuplication);
        ExecTest(GetContainsKeyProcessTime);
        ExecTest(GetContainsKeyProcessTimeIncludesDuplication);

        ExecTest(GetTryGetValueProcessTime);
        ExecTest(GetTryGetValueProcessTimeNotExistKey);
        ExecTest(GetValueWithContainsKeyProcessTime);
        ExecTest(GetValueWithContainsKeyProcessTimeNotExistKey);

        OutputResult();
    }

    /// <Summary>
    /// 引数のテスト用メソッドを実行し、実行時間を記録して表示する
    /// </Summary>
    void ExecTest(TestFunc func)
    {
        string methodName = func.Method.Name;
        Dictionary<int, DictionaryTestInfo> resultDict = new();

        foreach (int processCount in ProcessCountArray)
        {
            DictionaryTestInfo info = new();
            double resultSum = 0;
            for (int i = 0; i < _averageCount; i++)
            {
                double resultTime = func(processCount, _setCapacity);
                info.CheckTime(resultTime);
                resultSum += resultTime;
            }

            double result = resultSum / _averageCount;
            info.SetAverageTime(result);
            resultDict.TryAdd(processCount, info);

            var outputInfo = new DictionaryTestOutputInfo(methodName, processCount, _averageCount, info.AverageTime, info.MaxTime, info.MinTime);
            string message = GetOutputText(outputInfo);
            _outputMessages.Add(message);
        }

        ShowResult(methodName, resultDict);
    }

    /// <Summary>
    /// DictionaryのTryAddを使った追加を行うメソッド
    /// </Summary>
    double GetTryAddProcessTime(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = new();
        if (isSetCapacity)
        {
            testDict = new(processCount);
        }
        DateTime startDate = DateTime.Now;

        for (int i = 0; i < processCount; i++)
        {
            testDict.TryAdd(i, 0);
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// DictionaryのTryAddを使った追加を行うメソッド
    /// 重複時は値の更新を行う
    /// </Summary>
    double GetTryAddProcessTimeIncludesDuplication(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = new();
        if (isSetCapacity)
        {
            testDict = new(processCount);
        }

        // 重複対象のキーを事前に追加(測定区間外)
        for (int i = 0; i < processCount; i++)
        {
            if (CheckDuplicateCandidate(i))
            {
                testDict.TryAdd(i, 1);
            }
        }

        DateTime startDate = DateTime.Now;
        for (int i = 0; i < processCount; i++)
        {
            bool addResult = testDict.TryAdd(i, 0);
            if (!addResult)
            {
                testDict[i] = 0;
            }
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// 重複するキーかどうかの確認
    /// 重複する場合はTrueを返す
    /// </Summary>
    bool CheckDuplicateCandidate(int keyIndex)
    {
        int checkTarget = GetFirstDigitNum(keyIndex);
        foreach (int candidate in DuplicateCandidate)
        {
            if (checkTarget == candidate)
            {
                return true;
            }
        }
        return false;
    }

    /// <Summary>
    /// 引数の数値の1桁目の数値を返すメソッド
    /// </Summary>
    int GetFirstDigitNum(int num)
    {
        return num % 10;
    }

    /// <Summary>
    /// DictionaryのContainsKeyで確認後にAddを使った追加を行うメソッド
    /// </Summary>
    double GetContainsKeyProcessTime(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = new();
        if (isSetCapacity)
        {
            testDict = new(processCount);
        }
        DateTime startDate = DateTime.Now;

        for (int i = 0; i < processCount; i++)
        {
            if (testDict.ContainsKey(i))
            {
                testDict[i] = 0;
            }
            else
            {
                testDict.Add(i, 0);
            }
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// DictionaryのContainsKeyで確認後にAddを使った追加を行うメソッド
    /// 重複時は値の更新を行う
    /// </Summary>
    double GetContainsKeyProcessTimeIncludesDuplication(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = new();
        if (isSetCapacity)
        {
            testDict = new(processCount);
        }

        // 重複対象のキーを事前に追加(測定区間外)
        for (int i = 0; i < processCount; i++)
        {
            if (CheckDuplicateCandidate(i))
            {
                if (testDict.ContainsKey(i))
                {
                    testDict[i] = 1;
                }
                else
                {
                    testDict.Add(i, 1);
                }
            }
        }

        DateTime startDate = DateTime.Now;
        for (int i = 0; i < processCount; i++)
        {
            if (testDict.ContainsKey(i))
            {
                testDict[i] = 0;
            }
            else
            {
                testDict.Add(i, 0);
            }
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// DictionaryのTryGetValueを使った値の取得を行うメソッド
    /// </Summary>
    double GetTryGetValueProcessTime(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = GetTestDict(processCount);
        DateTime startDate = DateTime.Now;

        for (int i = 0; i < processCount; i++)
        {
            testDict.TryGetValue(i, out int value);
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// DictionaryのTryGetValueを使った値の取得を行うメソッド
    /// 存在しないキーを指定する
    /// </Summary>
    double GetTryGetValueProcessTimeNotExistKey(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = GetTestDict(processCount);
        DateTime startDate = DateTime.Now;

        for (int i = 0; i < processCount; i++)
        {
            int key = i + processCount;
            testDict.TryGetValue(i, out int value);
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// DictionaryのContainsKeyで確認後に値の取得を行うメソッド
    /// </Summary>
    double GetValueWithContainsKeyProcessTime(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = GetTestDict(processCount);
        DateTime startDate = DateTime.Now;

        for (int i = 0; i < processCount; i++)
        {
            if (testDict.ContainsKey(i))
            {
                var value = testDict[i];
            }
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// DictionaryのContainsKeyで確認後に値の取得を行うメソッド
    /// 存在しないキーを指定する
    /// </Summary>
    double GetValueWithContainsKeyProcessTimeNotExistKey(int processCount, bool isSetCapacity = false)
    {
        Dictionary<int, int> testDict = GetTestDict(processCount);
        DateTime startDate = DateTime.Now;

        for (int i = 0; i < processCount; i++)
        {
            int key = i + processCount;
            if (testDict.ContainsKey(i))
            {
                var value = testDict[i];
            }
        }

        DateTime endDate = DateTime.Now;
        TimeSpan span = endDate - startDate;
        double result = span.TotalMilliseconds;
        return result;
    }

    /// <Summary>
    /// 引数のデータ数に応じたテスト用の辞書を作成して返す
    /// </Summary>
    Dictionary<int, int> GetTestDict(int processCount)
    {
        Dictionary<int, int> testDict = new();
        for (int i = 0; i < processCount; i++)
        {
            testDict.TryAdd(i, 1);
        }
        return testDict;
    }

    /// <Summary>
    /// 引数の辞書に格納された処理結果を出力する
    /// </Summary>
    void ShowResult(string methodName, Dictionary<int, DictionaryTestInfo> resultDict)
    {
        Debug.Log($"{methodName}の処理結果 ({_averageCount}回の試行平均値) || キャパシティ設定 : {_setCapacity}");
        foreach (var pair in resultDict)
        {
            Debug.Log($"辞書のサイズ(処理回数) : {pair.Key} || 平均処理時間 : {pair.Value.AverageTime}ms || 最大処理時間 : {pair.Value.MaxTime}ms || 最小処理時間 : {pair.Value.MinTime}ms");
        }
    }

    /// <Summary>
    /// 処理結果をファイルに出力する
    /// </Summary>
    void OutputResult()
    {
        string header = GetOutputHeader();
        System.Text.StringBuilder sb = new();
        sb.Append(header).Append(Environment.NewLine);

        foreach (var msg in _outputMessages)
        {
            sb.Append(msg).Append(Environment.NewLine);
        }

        string output = sb.ToString();
        string folder = $"{Directory.GetCurrentDirectory()}/Assets";
        string fileName = "dictionary_output.csv";

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(folder, fileName)))
        {
            outputFile.WriteLine(output);
        }
    }

    /// <Summary>
    /// ファイル出力する際のヘッダーを返すメソッド
    /// </Summary>
    string GetOutputHeader()
    {
        string header = "";
        System.Text.StringBuilder sb = new();
        sb.Append("メソッド名").Append(",");
        sb.Append("データ数").Append(",");
        sb.Append("試行回数").Append(",");
        sb.Append("平均処理時間(ms)").Append(",");
        sb.Append("最大処理時間(ms)").Append(",");
        sb.Append("最小処理時間(ms)").Append(",");
        header = sb.ToString();

        return header;
    }

    /// <Summary>
    /// 結果の情報から、ファイルに出力する文字列を取得する
    /// </Summary>
    string GetOutputText(DictionaryTestOutputInfo info)
    {
        string msg = "";

        System.Text.StringBuilder sb = new();
        sb.Append(info._methodName).Append(",");
        sb.Append(info._dataSize).Append(",");
        sb.Append(info._average).Append(",");
        sb.Append(info._processTime).Append(",");
        sb.Append(info._processTimeMax).Append(",");
        sb.Append(info._processTimeMin).Append(",");
        msg = sb.ToString();

        return msg;
    }
}
