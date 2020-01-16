using System;
using System.ComponentModel;
using UiPathCloudAPISharp.Models;

namespace UiPathCloudAPISharp.Managers
{
    public delegate void WaitReadyJobCompletedEventHandler(object sender, WaitReadyJobCompletedEventArgs e);

    public class WaitReadyJobCompletedEventArgs
        //: AsyncCompletedEventArgs
    {
        //public WaitReadyJobCompletedEventArgs(Exception error, bool cancelled, object userState) : base(error, cancelled, userState)
        //{
        //}

        public JobWithArguments ReadyJob { get; set; }
    }
}