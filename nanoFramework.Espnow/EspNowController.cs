//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Runtime.Events;
using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Espnow
{
    /// <summary>
    /// ESP-NOW controller class.
    /// </summary>
    public class EspNowController : IDisposable
    {
        // keep in sync with nf-interpreter:src/HAL/Include/nanoHAL_v2.h
        private const int EVENT_ESPNOW = 140;

        /// <summary>
        /// Broadcast peer MAC address.
        /// </summary>
        public static readonly byte[] BROADCASTMAC = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        /// <summary>
        /// DataSent event handler type definition.
        /// </summary>
        public delegate void DataSendEventHandler(object sender, DataSentEventArgs eventArgs);

        /// <summary>
        /// Event raised after sending completed.
        /// </summary>
        public event DataSendEventHandler DataSent;

        /// <summary>
        /// DataReceived event handler type definition.
        /// </summary>
        public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs eventArgs);

        /// <summary>
        /// Event raised when data received.
        /// </summary>
        public event DataReceivedEventHandler DataReceived;

        private bool isDisposed;
        private EspNowEventHandler eventHandler;

        /// <summary>
        /// Controller.
        /// </summary>
        public EspNowController()
        {
            // Add a native event processor.
            eventHandler = new EspNowEventHandler(this);
            EventSink.AddEventProcessor((EventCategory)EVENT_ESPNOW, eventHandler);
            EventSink.AddEventListener((EventCategory)EVENT_ESPNOW, eventHandler);

            var nret = NativeInitialize();
            if (nret != 0)
            {
                throw new EspNowException(nret);
            }
        }

        /// <summary>
        /// Add peer to which data will be sent.
        /// </summary>
        /// <param name="peerMac">MAC address of peer. Use BROADCASTMAC for broadcasting.</param>
        /// <param name="channel">WiFi channel to be used.</param>
        public void AddPeer(byte[] peerMac, byte channel)
        {
            var nret = NativeEspNowAddPeer(peerMac, channel);
            if (nret != 0)
            {
                throw new EspNowException(nret);
            }
        }

        /// <summary>
        /// Send data to already registered peer.
        /// </summary>
        /// <param name="peerMac">MAC address of already added peer.</param>
        /// <param name="data">Data to be sent.</param>
        /// <param name="dataLen">Length of data.</param>
        public void Send(byte[] peerMac, byte[] data, int dataLen)
        {
            var nret = NativeEspNowSend(peerMac, data, dataLen);
            if (nret != 0)
            {
                throw new EspNowException(nret);
            }
        }

        private void OnDataReceived(byte[] peerMac, byte[] data, int dataLen)
        {
            var eh = this.DataReceived;
            if (eh != null)
            {
                eh(this, new DataReceivedEventArgs(peerMac, data, dataLen));
            }
        }

        private void OnDataSent(byte[] peerMac, int sendStatus)
        {
            var eh = this.DataSent;
            if (eh != null)
            {
                eh(this, new DataSentEventArgs(peerMac, sendStatus));
            }
        }

        /// <summary>
        /// Dispose()
        /// </summary>
        /// <param name="isDisposing">false on destructor call.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (eventHandler != null)
                    {
                        EventSink.RemoveEventProcessor((EventCategory)EVENT_ESPNOW, eventHandler);
                    }
                }

                eventHandler = null;

                NativeDispose(isDisposing);

                isDisposed = true;
            }
        }

        /// <summary>
        /// Destructor to assure Dispose will be called.
        /// </summary>
        ~EspNowController()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose()
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        internal class EspNowEventHandler : IEventProcessor, IEventListener
        {
            // keep in sync with nf-interpreter:targets/ESP32/_nanoCLR/nanoFramework.Espnow/nanoFramework_espnow_native.h
            private const int EVENT_ESPNOW_DATASENT = 1;
            private const int EVENT_ESPNOW_DATARECV = 2;

            private EspNowController controllerInstance;

            public EspNowEventHandler(EspNowController controllerInstance)
            {
                this.controllerInstance = controllerInstance;
            }

            public void InitializeForEventSource()
            {
                // no op
            }

            public bool OnEvent(BaseEvent ev)
            {
                bool ret = false;

                var dataRecvEvent = ev as DataRecvEventInternal;
                if (dataRecvEvent != null)
                {
                    controllerInstance.OnDataReceived(dataRecvEvent.PeerMac, dataRecvEvent.Data, dataRecvEvent.DataLen);
                    ret = true;
                }
                else
                {
                    var dataSentEvent = ev as DataSentEventInternal;
                    if (dataSentEvent != null)
                    {
                        controllerInstance.OnDataSent(dataSentEvent.PeerMac, dataSentEvent.Status);
                        ret = true;
                    }
                }

                return ret;
            }

            /// <summary>
            /// Native event processor
            /// </summary>
            /// <param name="data1"></param>
            /// <param name="data2"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            extern public BaseEvent ProcessEvent(uint data1, uint data2, DateTime time);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeInitialize();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeDispose(bool isDisposing);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_init()
        //private extern int NativeEspNowInit();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_deinit()
        //private extern int NativeEspNowDeinit();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_get_version(uint32_t *version)
        //private extern int NativeEspNowGetVersion(ref int version);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_register_recv_cb(esp_now_recv_cb_t cb)
        //private extern int NativeEspNowRegisterRecvCb(RegisterRecvDelegate cb);
        //private delegate void RegisterRecvDelegate(byte[] peerMac, byte[] data, int dataLen);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_unregister_recv_cb(void)
        //private extern int NativeEspNowUnregisterRecvCb();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_register_send_cb(esp_now_send_cb_t cb)
        //private extern int NativeEspNowRegisterSendCb(RegisterSendDelegate cb);
        //private delegate void RegisterSendDelegate(byte[] peerMac, int sendStatus);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_unregister_send_cb(void)
        //private extern int NativeEspNowUnregisterSendCb();

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_send(const uint8_t *peer_addr, const uint8_t *data, size_t len)
        private extern int NativeEspNowSend(byte[] peerMac, byte[] data, int dataLen);

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_add_peer(const esp_now_peer_info_t *peer)
        private extern int NativeEspNowAddPeer(byte[] peerMac, byte channel);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_now_set_pmk()
        //private extern int NativeEspNowSetPmk(int gpio, int rmtBufferSize);


        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// bool esp_now_is_peer_exist(const uint8_t *peer_addr)
        //private extern bool NativeEspNowIsPeerExist(byte[] peerMac);

        // esp_err_t esp_now_del_peer(const uint8_t *peer_addr)
        // esp_err_t esp_now_mod_peer(const esp_now_peer_info_t *peer)
        // esp_err_t esp_now_get_peer(const uint8_t *peer_addr, esp_now_peer_info_t *peer)
        // esp_err_t esp_now_fetch_peer(bool from_head, esp_now_peer_info_t *peer)
        // esp_err_t esp_now_get_peer_num(esp_now_peer_num_t *num)
        // esp_err_t esp_now_set_pmk(const uint8_t *pmk)
        // esp_err_t esp_now_set_wake_window(uint16_t window)

    }

    internal class DataSentEventInternal : BaseEvent
    {
        // these fields are set on native side
#pragma warning disable 0649
        public byte[] PeerMac;
        public int Status;
#pragma warning restore 0649

    }

    internal class DataRecvEventInternal : BaseEvent
    {
        // these fields are set on native side
#pragma warning disable 0649
        public byte[] PeerMac;
        public byte[] Data;
        public int DataLen;
#pragma warning restore 0649
    }

}
