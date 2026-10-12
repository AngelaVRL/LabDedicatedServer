using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class LatencyExperiment : MonoBehaviour
{
    private const string OutputFileName = "experimentA.csv";
    private const string CsvHeader = "intervalMs,avgRttMs,validSamples,systemRequestsPerSecond";
    private const int RequestsPerInstanceCycle = 2;
    private const float MillisecondsPerSecond = 1000f;

    [SerializeField] private ServerSettings settings;
    [Min(1)]
    [SerializeField] private int requestCount = 100;
    [Min(1)]
    [SerializeField] private int instanceCount = 2;
    [SerializeField] private int[] intervalsMs = { 50, 200, 1000 };

    private void Start()
    {
        if (settings == null || intervalsMs == null || intervalsMs.Length == 0)
        {
            Debug.LogError("LatencyExperiment needs settings and at least one interval.", this);
            enabled = false;
            return;
        }

        StartCoroutine(RunExperiment());
    }

    private IEnumerator RunExperiment()
    {
        string remoteUrl = settings.BuildPlayerUrl(settings.RemotePlayerId);
        yield return PositionApi.Post(remoteUrl, Vector3.zero);

        List<string> lines = new List<string> { CsvHeader };

        foreach (int intervalMs in intervalsMs)
        {
            List<float> samplesMs = new List<float>();

            for (int i = 0; i < requestCount; i++)
            {
                yield return MeasureGet(remoteUrl, samplesMs);
                yield return new WaitForSecondsRealtime(intervalMs / MillisecondsPerSecond);
            }

            float averageMs = Average(samplesMs);
            float cycleSeconds = (intervalMs + averageMs) / MillisecondsPerSecond;
            float systemRequestsPerSecond = instanceCount * RequestsPerInstanceCycle / cycleSeconds;

            lines.Add(string.Format(CultureInfo.InvariantCulture,
                "{0},{1:F2},{2},{3:F2}", intervalMs, averageMs, samplesMs.Count, systemRequestsPerSecond));
            Debug.Log($"Interval {intervalMs} ms: average RTT {averageMs:F2} ms ({samplesMs.Count} valid samples)");
        }

        ExperimentResultWriter.Save(OutputFileName, lines);
    }

    private IEnumerator MeasureGet(string url, List<float> samplesMs)
    {
        float start = Time.realtimeSinceStartup;

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            samplesMs.Add((Time.realtimeSinceStartup - start) * MillisecondsPerSecond);
        }
    }


    private static float Average(List<float> values)
    {
        if (values.Count == 0)
        {
            return 0f;
        }

        float sum = 0f;
        foreach (float value in values)
        {
            sum += value;
        }

        return sum / values.Count;
    }
}