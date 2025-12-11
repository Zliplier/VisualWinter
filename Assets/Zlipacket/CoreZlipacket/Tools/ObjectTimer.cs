using UnityEngine;
using UnityEngine.Events;

namespace Zlipacket.CoreZlipacket.Tools
{
    public class ObjectTimer : MonoBehaviour
    {
        [SerializeField] private float duration;

        public float timeElapsed { get; private set; } = 0f;
        public float percentage { get; private set; } = 0f;
        public bool isRunning {get; private set;} = false;
        
        [Header("Events")]
        public UnityEvent onTimerStart;
        public UnityEvent onTimerFinished;
        public UnityEvent onTimerPaused;
        public UnityEvent onTimerReset;

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }
        
        public void StartTimer()
        {
            onTimerStart?.Invoke();
            isRunning = true;
        }

        public void PauseTimer()
        {
            onTimerPaused?.Invoke();
            isRunning = false;
        }

        public void ResetTimer()
        {
            onTimerReset?.Invoke();
            timeElapsed = 0f;
            percentage = 0f;
            isRunning = false;
        }
        
        private void Update()
        {
            if (!isRunning) return;
            
            CalculatePercentage();
            
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= duration)
            {
                timeElapsed = duration;
                TimerFinished();
                isRunning = false;
            }
        }

        private void CalculatePercentage()
        {
            if (duration > 0)
            {
                percentage = timeElapsed / duration;
            }
        }

        private void TimerFinished()
        {
            onTimerFinished?.Invoke();
        }
    }
}