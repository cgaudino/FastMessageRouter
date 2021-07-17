using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastMessageRouter.Samples
{
    public class SampleMessageHandler : MonoBehaviour
    {
        private void Start()
        {
            MessageRouter.Instance.BindMessageHandler<PrintLogMessage>(PrintLog);
            MessageRouter.Instance.BindMessageHandler<PrintWarningMessage>(PrintWarning);
        }

        private void OnDestroy()
        {
            MessageRouter.Instance.RemoveMessageHandler<PrintLogMessage>(PrintLog);
            MessageRouter.Instance.RemoveMessageHandler<PrintWarningMessage>(PrintWarning);
        }

        private void PrintLog(PrintLogMessage message)
        {
            Debug.Log($"Received a log message with text: {message.logText}");
        }

        private void PrintWarning(PrintWarningMessage message)
        {
            Debug.Log($"Received a warning message with text: {message.warningText}");
        }
    }
}
