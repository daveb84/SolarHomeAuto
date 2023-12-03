namespace SolarHomeAuto.Domain
{
    public static class DateTimeNow
    {
        private static DateTime? overrideDate;
        private static bool disableDelay;
        private static Action onDelayCallback;

        public static DateTime UtcNow
        {
            get
            {
                return overrideDate ?? DateTime.UtcNow;
            }
        }

        public static void SetTestTimeNowOverride(DateTime now)
        {
            overrideDate = now;
        }

        public static void SetTestDelayOverride(bool disableDelay = false, Action onDelayCallback = null)
        {
            DateTimeNow.disableDelay = disableDelay;
            DateTimeNow.onDelayCallback = onDelayCallback;
        }

        public static void ResetTestOverrides()
        {
            overrideDate = null;
            disableDelay = false;
            onDelayCallback = null;
        }

        public static Task Delay(int milliSeconds, CancellationToken cancellationToken = default)
        {
            if (disableDelay)
            {
                if (overrideDate.HasValue)
                {
                    overrideDate += TimeSpan.FromMilliseconds(milliSeconds);
                }

                onDelayCallback?.Invoke();

                return Task.CompletedTask;
            }

            return Task.Delay(milliSeconds, cancellationToken);
        }
    }
}
