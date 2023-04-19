using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace Client1
{
    public class Mqtt
    {
        static MqttClient _mqttClient;

        public MqttClient Client => _mqttClient;
        public string ClientId => _mqttClient.ClientId;
        

        public event EventHandler ConnectionChanged;
        public event EventHandler MessageReceived;

        protected string GetDefaultSubscribeTopic(string prefix)
        {
            return prefix + '/' + _mqttClient.ClientId;
        }
        protected virtual void OnConnectError(Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        public bool IsConnected => _mqttClient != null && _mqttClient.IsConnected;

        //const string _cid = "D34F4441";
        const string _topic = "aks";
        //const string host = "localhost";
        //const string host = "system.aks.vn";
        const string host = "192.168.137.206";
        const int port = 8090;

        public virtual void Connect(string clientId = null)
        {

            if (_mqttClient != null && _mqttClient.IsConnected) { return; }

            Console.WriteLine("Connect to " + host + "...");
            try
            {
                _mqttClient = new MqttClient(
                    host,
                    port,
                    false,
                    MqttSslProtocols.None,
                    null,
                    null
                );
                _mqttClient.MqttMsgPublishReceived += (s, e) => {
                    var payload = e.Message;
                    //Response = DataContext.Parse<Vst.Context>(payload);

                    //MessageReceived?.Invoke(this, EventArgs.Empty);
                };

                _mqttClient.ConnectionClosed += (s, e) => RaiseConnectionChanged();
                if (clientId == null)
                {
                    //clientId = _cid;
                    clientId = Guid.NewGuid().ToString();
                }
                _mqttClient.Connect(clientId);
                if (_mqttClient.IsConnected)
                {
                    Console.WriteLine("Server Connected");
                    //RaiseConnectionChanged();
                    //Subscribe(GetDefaultSubscribeTopic("response/default"));
                }
            }
            catch (Exception e)
            {
                OnConnectError(e);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (_mqttClient != null && _mqttClient.IsConnected)
                {
                    _mqttClient.Disconnect();
                }
            }
            catch
            {
            }
        }
        protected virtual void RaiseConnectionChanged()
        {
            ConnectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Publish(string topic, string url)
        {
            this.Publish(topic, url, new { });
        }
        public void Publish(string topic, string url, object value)
        {
            //if (_mqttClient != null)
            //{
            //    if (value == null) { value = 0; }
            //    var context = new Vst.Context
            //    {
            //        Url = url,
            //        Value = value,
            //    };
            //    Publish(topic ?? _topic, context);
            //}
        }
        //public virtual void Publish(string topic, Vst.Context context)
        //{
        //    if (_mqttClient != null)
        //    {
        //        _mqttClient.Publish(topic ?? _topic, context.ToString().UTF8());
        //    }
        //}
        public void Subscribe(string topic)
        {
            _mqttClient.Subscribe(new string[] { topic }, new byte[] { 0 });
        }
        public void ClearEvent() => MessageReceived = null;
    }
}
