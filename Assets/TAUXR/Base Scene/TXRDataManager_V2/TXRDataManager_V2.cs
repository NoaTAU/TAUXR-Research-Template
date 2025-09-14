// TXRDataManager_V2.cs
// Builds schemas, opens CSVs, runs collectors every FixedUpdate.
// Researchers use: LogCustom(...) and the inspector list of custom transforms.

using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace TXRData
{
    public sealed class TXRDataManager_V2 : TXRSingleton<TXRDataManager_V2>
    {


        [Header("Editor Export")]
        public bool exportInEditor = false;

        [ShowIf(nameof(exportInEditor))]
        public string saveFilePath;  // if null/empty, falls back to tmp

        [Header("Output")]
        //public string outputFolderName = "TXR_Logs";
        private string sessionTime;
        public string SessionTime => sessionTime; // read-only property
        public bool appendIfFilesExist = false;
        public string csvDelimiter = ",";

        [Header("What to record (ContinuousData)")]
        public RecordingOptions recordingOptions = new RecordingOptions();

        [Header("FaceExpressions.csv")]
        public bool recordFaceExpressions = true;

        // paths
        private string _rootDir;

        // schemas
        private ColumnIndex _continuousSchema;
        private ColumnIndex _faceSchema;

        // row buffers
        private RowBuffer _continuousRow;
        private RowBuffer _faceRow;

        // writers
        private CsvRowWriter _continuousWriter;
        private CsvRowWriter _faceWriter;

        // collectors
        private readonly List<IContinuousCollector> _continuousCollectors = new List<IContinuousCollector>();
        private OVRFaceCollector _faceCollector;

        private void Awake()
        {

            // 0) session time suffix
            sessionTime = DateTime.UtcNow.ToString("yyyy.MM.dd_HH-mm");

            // 1) Output directory
            if (Application.isEditor && exportInEditor)
            {
                if (!string.IsNullOrWhiteSpace(saveFilePath))
                {
                    _rootDir = saveFilePath;
                    Directory.CreateDirectory(_rootDir);
                }
                else
                {
                    _rootDir = Path.Combine(Path.GetTempPath(), "TXR_EditorLogs");
                }
            }
            else
            {
                _rootDir = Application.persistentDataPath;
                //_rootDir = Path.Combine(Application.persistentDataPath, outputFolderName);
                //Directory.CreateDirectory(_rootDir);
            }


            // 2) Build schemas
            var cont = SchemaFactories.BuildContinuousDataV2(recordingOptions);  // (schema, counts, flags)
            _continuousSchema = cont.schema;

            var face = SchemaFactories.BuildFaceExpressionsV2();                 // (schema, counts)
            _faceSchema = face.schema;

            // 3) Writers
            string contPath = Path.Combine(_rootDir, $"{sessionTime}_ContinuousData.csv");
            _continuousWriter = new CsvRowWriter(contPath, csvDelimiter, null, appendIfFilesExist);

            if (recordFaceExpressions)
            {
                string facePath = Path.Combine(_rootDir, $"{sessionTime}_FaceExpressionData.csv");
                _faceWriter = new CsvRowWriter(facePath, csvDelimiter, null, appendIfFilesExist);
            }

            // 4) Row buffers
            _continuousRow = new RowBuffer(_continuousSchema);
            _faceRow = recordFaceExpressions ? new RowBuffer(_faceSchema) : null;

            // 5) Collectors for ContinuousData
            if (recordingOptions.includeNodes) _continuousCollectors.Add(new OVRNodesCollector());
            if (recordingOptions.includeEyes) _continuousCollectors.Add(new OVREyesCollector());
            if (recordingOptions.includeHands) _continuousCollectors.Add(new OVRHandsCollector());
            if (recordingOptions.includeBody) _continuousCollectors.Add(new OVRBodyCollector());
            if (recordingOptions.includeRecenter) _continuousCollectors.Add(new RecenterCollector());
            if (recordingOptions.customTransformsToRecord != null &&
                recordingOptions.customTransformsToRecord.Count > 0)
                _continuousCollectors.Add(new CustomTransformsCollector());

            foreach (var c in _continuousCollectors)
                c.Configure(_continuousSchema, recordingOptions);

            if (recordFaceExpressions)
            {
                _faceCollector = new OVRFaceCollector();
                _faceCollector.Configure(_faceSchema, recordingOptions);
            }

            // 6) Custom data tables: set base directory + delimiter once
            CustomCsvFromDataClass.Initialize(_rootDir, csvDelimiter);           // :contentReference[oaicite:8]{index=8}
        }

        private void FixedUpdate()
        {
            float t = Time.realtimeSinceStartup;

            // ContinuousData row
            _continuousRow.Clear();
            _continuousRow.TrySet("timeSinceStartup", t);                        // friendly setter :contentReference[oaicite:9]{index=9}
            for (int i = 0; i < _continuousCollectors.Count; i++)
                _continuousCollectors[i].Collect(_continuousRow, t);

            // write row
            _continuousWriter.WriteRow(_continuousSchema,
                                       _continuousRow.ValuesArray,
                                       _continuousRow.ColumnIsSetMask);         // WriteRow signature 

            // FaceExpressions row
            if (recordFaceExpressions && _faceCollector != null)
            {
                _faceRow.Clear();
                _faceRow.TrySet("timeSinceStartup", t);
                _faceCollector.Collect(_faceRow, t);

                _faceWriter.WriteRow(_faceSchema,
                                     _faceRow.ValuesArray,
                                     _faceRow.ColumnIsSetMask);
            }
        }

        private void OnDestroy()
        {
            // collectors
            for (int i = 0; i < _continuousCollectors.Count; i++)
            {
                try { _continuousCollectors[i].Dispose(); } catch { }
            }
            _continuousCollectors.Clear();

            // writers
            try { _continuousWriter?.Dispose(); } catch { }
            try { _faceWriter?.Dispose(); } catch { }

            // custom tables
            try { CustomCsvFromDataClass.CloseAll(); } catch { }
        }

        // ---------- minimal API for researchers ----------

        // Create & write a row to <TableName>.csv using a custom data class instance.
        public void LogCustom(CustomDataClass data)
        {
            if (data == null) return;
            CustomCsvFromDataClass.Write(data);
        }

        // Overload that builds the object on demand (avoids allocations at call site).
        public void LogCustom(Func<CustomDataClass> make)
        {
            if (make == null) return;
            var inst = make();
            if (inst == null) return;
            CustomCsvFromDataClass.Write(inst);
        }

        public string GetOutputDirectory() => _rootDir;
    }
}
