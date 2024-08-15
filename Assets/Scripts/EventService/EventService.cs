using System;
using System.Collections;
using System.Collections.Generic;
using EventService.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace EventService
{
    public class EventService : MonoBehaviour
    {
        private float _cooldownBeforeSend = 3f;
        private string _serverUrl = "";
        private bool _isCooldownStart = false;
        private List<AnalyticEventData> _analyticEventDataQueue = new List<AnalyticEventData>();

        private void Start()
        {
            if (_analyticEventDataQueue.Count != 0)
                SendEvent();
        }

        public void TrackEvent(string type, string data)
        {
            AnalyticEventData analyticEventData = new AnalyticEventData(type, data);
            _analyticEventDataQueue.Add(analyticEventData);

            if (!_isCooldownStart)
            {
                  SendEvent();
                  _isCooldownStart = true;
                  StartCoroutine(StartTimer());
            }
        }

        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(_cooldownBeforeSend);
            _isCooldownStart = false;
            
            if(_analyticEventDataQueue.Count != 0)
                SendEvent();
        }

        private void SendEvent()
        {
            foreach (AnalyticEventData analyticEventData in _analyticEventDataQueue)
            {
                StartCoroutine(SendEvent(analyticEventData));
            }
        }

        private IEnumerator SendEvent(AnalyticEventData analyticEventData)
        {
            string data = JsonUtility.ToJson(analyticEventData);
            
            using (UnityWebRequest request = UnityWebRequest.Post(_serverUrl, data))
            {
                yield return request.SendWebRequest();

                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    if (request.responseCode == 200)
                    {
                        _analyticEventDataQueue.Remove(analyticEventData);
                    }
                }
            }
        }
    }
}