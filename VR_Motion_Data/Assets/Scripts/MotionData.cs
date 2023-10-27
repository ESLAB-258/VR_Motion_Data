using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor;

public class MotionData : MonoBehaviour
{
    [HideInInspector]
    public string fileName/* = "Assets/CSV/HandData.csv"*/;

    enum E_SPORTS_NAME
    {
        BOWLING = 0,
        GOLF,
        WALKING,
    }
    enum E_GENDER
    {
        MALE = 0,
        FEMALE,
        OTHERS,
    }

    [SerializeField]
    Transform[] devices;

    [SerializeField]
    Text timeText;

    [SerializeField]
    Text[] posTexts;

    [SerializeField]
    Text[] rotTexts;

    [SerializeField]
    E_SPORTS_NAME sports = E_SPORTS_NAME.BOWLING;

    [SerializeField]
    E_GENDER gender = E_GENDER.MALE;

    [SerializeField]
    string generation;

    [SerializeField]
    string age;

    [SerializeField]
    string subjectName;

    [SerializeField]
    string date;

    [SerializeField]
    string actNumber;

    FileStream fs;
    StreamWriter sw;

    string timerStr = @"00:00:00.000";
    float totalSeconds;

    private void OnEnable()
    {
        string sportsStr = null;

        switch(sports)
        {
            case E_SPORTS_NAME.BOWLING:
                sportsStr = "Bowling";
                break;
            case E_SPORTS_NAME.GOLF:
                sportsStr = "Golf";
                break;
            case E_SPORTS_NAME.WALKING:
                sportsStr = "Walking";
                break;

        }

        string genderStr = null;

        switch (gender)
        {
            case E_GENDER.MALE:
                genderStr = "Male";
                break;
            case E_GENDER.FEMALE:
                genderStr = "Female";
                break;
            case E_GENDER.OTHERS:
                genderStr = "Other";
                break;

        }

        if (string.IsNullOrEmpty(sportsStr) || string.IsNullOrEmpty(generation) || string.IsNullOrEmpty(genderStr) || string.IsNullOrEmpty(age) || 
            string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(actNumber))
        {
            Debug.LogError("You need to more information for making fileName.");
        }

        fileName = sportsStr + "_" + generation + "_" + genderStr + "_" + age + "_" + subjectName + "_" + date + "_" + actNumber;

        fs = new FileStream("Assets/CSV/" + fileName + ".csv", FileMode.Create, FileAccess.Write);
        sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

        string schema = null;

        string[] dataTypesStr = { "Pos", "Rot" };
        string[] bodyStr = { "HMD", "LeftController", "RightController", "Waist", "LeftHand", "RightHand", "LeftFoot", "RightFoot" };

        //isWriting = false;

        schema += "TimeStamp,";

        for (int i = 0; i < bodyStr.Length; i++)
        {
            for (int j = 0; j < dataTypesStr.Length; j++)
            {
                schema += bodyStr[i] + dataTypesStr[j];

                if (i != bodyStr.Length - 1 || j != dataTypesStr.Length - 1)
                {
                    schema += ",";
                }
            }
        }

        sw.WriteLine(schema);

        StartCoroutine(CoCollectMotionData());
    }

    IEnumerator CoCollectMotionData()
    {
        while (true)
        {
            timerStr = Timer();

            string data = null;

            int i = 0;

            data += timerStr + ",";

            timeText.text = timerStr;

            foreach (var device in devices)
            {
                Vector3 devicePos = device.position;
                Vector3 deviceRot = device.rotation.eulerAngles;

                data += devicePos.ToString("0.00") + "," + deviceRot.ToString("0.00");

                if (i != devices.Length - 1)
                {
                    data += ",";
                }

                posTexts[i].text = devicePos.ToString("0.00");
                rotTexts[i].text = deviceRot.ToString("0.00");

                i++;
            }

            //totalData += data;

            sw.WriteLine(data);
            //Debug.Log(data);

            yield return new WaitForFixedUpdate();
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        timerStr = Timer();

        string data = null;

        int i = 0;

        data += timerStr + ",";

        timeText.text = timerStr;

        foreach (var tracker in trackers)
        {
            Vector3 trackerPos = tracker.position;
            Vector3 trackerRot = tracker.rotation.eulerAngles;

            data += trackerPos.ToString("0.00") + "," + trackerRot.ToString("0.00");

            if (i != trackers.Length - 1)
            {
                data += ",";
            }

            posTexts[i].text = trackerPos.ToString("0.00");
            rotTexts[i].text = trackerRot.ToString("0.00");

            i++;
        }

        //totalData += data;

        sw.WriteLine(data);
        //Debug.Log(data);
    }*/

    string Timer()
    {
        totalSeconds += Time.deltaTime;
        TimeSpan timespan = TimeSpan.FromSeconds(totalSeconds);
        string timer = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

        return timer;
    }

    void OnApplicationQuit()
    {
        sw.Close();
        fs.Close();

        StopCoroutine(CoCollectMotionData());
    }
}
