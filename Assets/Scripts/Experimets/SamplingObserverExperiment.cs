using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SamplingObserverExperiment : MonoBehaviour
{
    private const string OutputFileName = "experimentB.csv";
    private const string CsvHeader = "intervalMs,distinctValuesSeen,expectedValues";
    private const float MillisecondsPerSecond = 1000f;

    [SerializeField] private ServerSettings settings;
    [SerializeField] private KeyCode startTrialKey = KeyCode.N;
    [Min(1)]
    [SerializeField] private int expectedValues = 20;
    [Min(1f)]
    [SerializeField] private float observationWindowSeconds = 5f;
    [SerializeField] private int[] intervalsMs = { 50, 200, 1000 };

    private readonly List<string> csvLines = new List<string> { CsvHeader };
    private int nextIntervalIndex;
    private bool isObserving;

    private void Start()
    {
        if (settings == null || intervalsMs == null || intervalsMs.Length == 0)
        {
            Debug.LogError("SamplingObserverExperiment needs settings and at least one interval.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        bool hasPendingTrials = nextIntervalIndex < intervalsMs.Length;

        if (Input.GetKeyDown(startTrialKey) && !isObserving && hasPendingTrials)
        {
            StartCoroutine(RunTrial(intervalsMs[nextIntervalIndex]));
        }
    }

    private IEnumerator RunTrial(int intervalMs)
    {
        isObserving = true;
        string url = settings.BuildPlayerUrl(settings.RemotePlayerId);
        HashSet<int> observedValues = new HashSet<int>();

        yield return PositionApi.Post(url, Vector3.zero);
        Debug.Log($"Trial {intervalMs} ms started. Trigger the burst on the other instance now.");

        float endTime = Time.realtimeSinceStartup + observationWindowSeconds;

        while (Time.realtimeSinceStartup < endTime)
        {
            yield return PositionApi.Get(url, position => RecordValue(position, observedValues));
            yield return new WaitForSecondsRealtime(intervalMs / MillisecondsPerSecond);
        }

        csvLines.Add(string.Format(CultureInfo.InvariantCulture,
            "{0},{1},{2}", intervalMs, observedValues.Count, expectedValues));
        Debug.Log($"Interval {intervalMs} ms: {observedValues.Count} of {expectedValues} values seen.");

        nextIntervalIndex++;
        if (nextIntervalIndex >= intervalsMs.Length)
        {
            ExperimentResultWriter.Save(OutputFileName, csvLines);
        }

        isObserving = false;
    }

    private void RecordValue(PositionData position, HashSet<int> observedValues)
    {
        int value = Mathf.RoundToInt(position.posX);

        if (value >= 1 && value <= expectedValues)
        {
            observedValues.Add(value);
        }
    }
}