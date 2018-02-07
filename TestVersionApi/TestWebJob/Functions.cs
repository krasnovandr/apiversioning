﻿using System.IO;

using Microsoft.Azure.WebJobs;

namespace TestWebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([ServiceBusTrigger("hellotopic", "FirstVersion")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }
    }
}
