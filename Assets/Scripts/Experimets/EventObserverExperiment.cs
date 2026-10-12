using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EventObserverExperiment : MonoBehaviour
{
    private const string OutputFileName = "experimentC.csv";
    private const string CsvHeader = "intervalMs,eventsReceived,expectedEvents,inOrder,duplicates";
    private const float MillisecondsPerSecond = 1000f;

    [SerializeField] private ServerSettings settings;
    [SerializeField] private KeyCode startTrialKey = KeyCode.M;
    [Min(1)]
    [SerializeField] private int expectedEvents = 20;
    [Min(1f)]
    [SerializeField] private float observationWindowSeconds = 10f;
    [SerializeField] private int[] intervalsMs = { 50, 200, 1000 };

    private readonly List<string> csvLines = new List<string> { CsvHeader };
    private int nextIntervalIndex;
    private bool isObserving;

    private void Start()
    {
        if (settings == null || intervalsMs == null || intervalsMs.Length == 0)
        {
            Debug.LogError("EventObserverExperiment needs settings and at least one interval.", this);
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
        TrialRecorder recorder = new TrialRecorder();
        long lastSeq = 0;

        yield return EventApi.Get(settings.BuildEventsUrl(0), batch => lastSeq = batch.LastSeq);
        Debug.Log($"Trial {intervalMs} ms started. Trigger the burst on the other instance now.");

        float endTime = Time.realtimeSinceStartup + observationWindowSeconds;

        while (Time.realtimeSinceStartup < endTime)
        {
            yield return EventApi.Get(
                settings.BuildEventsUrl(lastSeq),
                batch => lastSeq = recorder.Record(batch, lastSeq, settings.LocalPlayerId));
            yield return new WaitForSecondsRealtime(intervalMs / MillisecondsPerSecond);
        }

        csvLines.Add(string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}",
            intervalMs, recorder.ReceivedCount, expectedEvents, recorder.IsInOrder, recorder.Duplicates));
        Debug.Log($"Interval {intervalMs} ms: {recorder.ReceivedCount} of {expectedEvents} events, " +
                  $"in order: {recorder.IsInOrder}, duplicates: {recorder.Duplicates}");

        nextIntervalIndex++;
        if (nextIntervalIndex >= intervalsMs.Length)
        {
            ExperimentResultWriter.Save(OutputFileName, csvLines);
        }

        isObserving = false;
    }

    private class TrialRecorder
    {
        private readonly HashSet<long> seenSeqs = new HashSet<long>();
        private int lastIndex;

        public int ReceivedCount { get; private set; }
        public int Duplicates { get; private set; }
        public bool IsInOrder { get; private set; } = true;

        public long Record(EventBatch batch, long currentLastSeq, string localPlayerId)
        {
            long newLastSeq = currentLastSeq;

            if (batch.Events == null)
            {
                return newLastSeq;
            }

            foreach (GameEvent gameEvent in batch.Events)
            {
                newLastSeq = System.Math.Max(newLastSeq, gameEvent.Seq);

                if (gameEvent.PlayerId == localPlayerId || gameEvent.Payload == null)
                {
                    continue;
                }

                if (!seenSeqs.Add(gameEvent.Seq))
                {
                    Duplicates++;
                    continue;
                }

                ReceivedCount++;

                if (gameEvent.Payload.Index <= lastIndex)
                {
                    IsInOrder = false;
                }

                lastIndex = gameEvent.Payload.Index;
            }

            return newLastSeq;
        }
    }
}