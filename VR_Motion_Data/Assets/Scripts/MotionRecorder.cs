#if UNITY_EDITOR

using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// This example shows how to set up a recording session via script, for an MP4 file.
    /// To use this example, add the MovieRecorderExample component to a GameObject.
    ///
    /// Enter the Play Mode to start the recording.
    /// The recording automatically stops when you exit the Play Mode or when you disable the component.
    ///
    /// This script saves the recording outputs in [Project Folder]/SampleRecordings.
    /// </summary>
    public class MotionRecorder : MonoBehaviour
    {
        RecorderController m_RecorderController;
        //public bool m_RecordAudio = true;
        internal MovieRecorderSettings m_Settings = null;

        public FileInfo OutputFile
        {
            get
            {
                var fileName = m_Settings.OutputFile + ".mp4";
                return new FileInfo(fileName);
            }
        }

        void OnEnable()
        {
            Initialize();
        }

        internal void Initialize()
        {
            MotionData motionData = GetComponent<MotionData>();

            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            var mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath + "/Recordings"));

            // Video
            m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            m_Settings.name = "Motion Data Recorder";
            m_Settings.Enabled = true;

            // This example performs an MP4 recording
            m_Settings.EncoderSettings = new CoreEncoderSettings
            {
                EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.Medium,
                Codec = CoreEncoderSettings.OutputCodec.MP4
            };
            m_Settings.CaptureAlpha = true;

            m_Settings.ImageInputSettings = new GameViewInputSettings
            {
                OutputWidth = 1920,
                OutputHeight = 1080
            };

            m_Settings.AudioInputSettings.PreserveAudio = false;

            // Simple file name (no wildcards) so that FileInfo constructor works in OutputFile getter.
            m_Settings.OutputFile = mediaOutputFolder.FullName + "/" + motionData.fileName;

            // Setup Recording
            controllerSettings.AddRecorderSettings(m_Settings);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 30.0f;

            RecorderOptions.VerboseMode = false;
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            //Debug.Log($"Started recording for file {OutputFile.FullName}");
        }

        void OnApplicationQuit()
        {
            m_RecorderController.StopRecording();
        }
    }
}

#endif
