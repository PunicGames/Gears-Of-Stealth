using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    public float alertTime = 60f;
    private float currentAlertTime;

    [HideInInspector] public bool isActive = false;

    public delegate void OnStartAlarm(Vector3 lastSeenPos);
    public OnStartAlarm onStartAlarm;

    public delegate void OnStopAlarm();
    public OnStopAlarm onStopAlarm;

    public delegate void OnTimeUpdated(float t);
    public OnTimeUpdated onTimeUpdated;

    [SerializeField]
    MeshRenderer floor;
    public Color colorON;
    public Color colorOFF;

    private void Start()
    {
        currentAlertTime = alertTime;
    }
    public void SoundAlarm(Vector3 playerLastSeenPos)
    {
        if (!isActive)
        {
            isActive = true;
            GetComponent<AudioSource>().Play();
            onStartAlarm?.Invoke(playerLastSeenPos);
            Invoke(nameof(StopAlarm), alertTime);
            InvokeRepeating(nameof(UpdateTime), 1, 1);
            floor.material.EnableKeyword("_EMISSION");
            floor.material.SetColor("_EmissionColor", colorON);
        }

    }
    private void StopAlarm()
    {
        if (isActive)
        {
            isActive = false;
            GetComponent<AudioSource>().Stop();
            CancelInvoke(nameof(UpdateTime));
            currentAlertTime = alertTime;
            onStopAlarm?.Invoke();
            floor.material.EnableKeyword("_EMISSION");
            floor.material.SetColor("_EmissionColor", colorOFF);
        }

    }
    private void UpdateTime()
    {
        currentAlertTime--;
        if (isActive)
            onTimeUpdated?.Invoke(currentAlertTime);
    }

}
