using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FastMessageRouter.Samples
{
    public class SampleMessageSender : MonoBehaviour
    {
        [SerializeField] private InputField _messageInputField = default;
        [SerializeField] private Button _sendLogMessage = default;
        [SerializeField] private Button _sendWarningMessage = default;

        private PrintLogMessage _logMessage;
        private PrintWarningMessage _warningMessage;

        private void Start()
        {
            _logMessage = new PrintLogMessage();
            _warningMessage = new PrintWarningMessage();

            _sendLogMessage.onClick.AddListener(SendLogMessage);
            _sendWarningMessage.onClick.AddListener(SendWarningMessage);
        }

        private void OnDestroy()
        {
            _sendLogMessage.onClick.RemoveListener(SendLogMessage);
            _sendWarningMessage.onClick.RemoveListener(SendWarningMessage);
        }

        private void SendLogMessage()
        {
            _logMessage.logText = _messageInputField.text;
            MessageRouter.Instance.RaiseMessage<PrintLogMessage>(_logMessage);
        }

        private void SendWarningMessage()
        {
            _warningMessage.warningText = _messageInputField.text;
            MessageRouter.Instance.RaiseMessage<PrintWarningMessage>(_warningMessage);
        }
    }
}
