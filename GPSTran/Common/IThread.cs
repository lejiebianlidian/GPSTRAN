using System.Threading;

namespace Common
{
  public  interface IThread
    {
        //ThreadState
        ThreadState ThreadStatus { get; }
        //Start thread
        int Start();
        //Tell thread to stop and return immediately .But maybe thread is not stopped immediately.GPS data may be lost
        void BeginStop();
        //Tell thread to suspend and return immediately .But maybe thread is not suspended immediately
        void BeginSuspend();
        //Tell thread to resume and return immediately .But maybe thread is not resumed immediately
        void BeginResume();
        //Tell thread to stop and wait until thread has finshed sending data or any other tasks.No gps data lost
        int Safe_Stop();

    }
}
