using System;
using Android.App;
using Android.OS;
using Android.Runtime;

namespace BiometricAuthentication.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static Activity CurrentActivity { get; private set; }

        //public static ApplicationState CurrentState { get; private set; }

        public static IApplication Application { get; set; }

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            //CurrentState = ApplicationState.Active;
            Application?.OnCreate();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
            //CurrentState = ApplicationState.Background;
            Application?.OnTerminate();
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CurrentActivity = activity;
            Application?.OnActivityCreated(activity, savedInstanceState);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            Application?.OnActivityDestroyed(activity);
        }

        public void OnActivityPaused(Activity activity)
        {
            Application?.OnActivityPaused(activity);
        }

        public void OnActivityResumed(Activity activity)
        {
            CurrentActivity = activity;
            Application?.OnActivityResumed(activity);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            Application?.OnActivitySaveInstanceState(activity, outState);
        }

        public void OnActivityStarted(Activity activity)
        {
            CurrentActivity = activity;
            Application?.OnActivityStarted(activity);
        }

        public void OnActivityStopped(Activity activity)
        {
            Application?.OnActivityStopped(activity);
        }
    }
}
