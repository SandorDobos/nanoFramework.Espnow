//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Runtime.CompilerServices;

namespace nanoFramework.Hardware.Esp32.EspNow
{
    /// <summary>
    /// ESP-NOW controller class.
    /// </summary>
    public class EspNowController : IDisposable
    {
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

        /// <summary>
        /// Controller.
        /// </summary>
        public EspNowController()
        {
            NativeEspNowInit();

            NativeEspNowRegisterRecvCb(RecvCb);
            NativeEspNowRegisterSendCb(SendCb);
        }

        /// <summary>
        /// Add peer to which data will be sent.
        /// </summary>
        /// <param name="peerMac">MAC address of peer. Use ff:ff:ff:ff:ff:ff for broadcasting.</param>
        /// <param name="channel">WiFi channel to be used.</param>
        /// <returns>esp_err_t values, see esp_now.h</returns>
        public int AddPeer(byte[] peerMac, byte channel)
        {
            int ret = NativeEspNowAddPeer(peerMac, channel);
            return ret;
        }

        /// <summary>
        /// Send data to already registered peer.
        /// </summary>
        /// <param name="peerMac">MAC address of already added peer.</param>
        /// <param name="data">Data to be sent.</param>
        /// <param name="dataLen">Length of data.</param>
        /// <returns>esp_err_t values, see esp_now.h</returns>
        public int Send(byte[] peerMac, byte[] data, int dataLen)
        {
            int ret = NativeEspNowSend(peerMac, data, dataLen);
            return ret;
        }

        private void RecvCb(byte[] peerMac, byte[] data, int dataLen)
        {
            var eh = this.DataReceived;
            if (eh != null)
            {
                eh(this, new DataReceivedEventArgs(peerMac, data, dataLen));
            }
        }

        private void SendCb(byte[] peerMac, int sendStatus)
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
                    NativeEspNowUnregisterRecvCb();
                    NativeEspNowUnregisterSendCb();
                    NativeEspNowDeinit();
                }
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

        private delegate void RegisterRecvDelegate(byte[] peerMac, byte[] data, int dataLen);
        private delegate void RegisterSendDelegate(byte[] peerMac, int sendStatus);

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_init()
        private extern int NativeEspNowInit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_deinit()
        private extern int NativeEspNowDeinit();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //// esp_err_t esp_now_get_version(uint32_t *version)
        //private extern int NativeEspNowGetVersion(ref int version);

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_register_recv_cb(esp_now_recv_cb_t cb)
        private extern int NativeEspNowRegisterRecvCb(RegisterRecvDelegate cb);

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_unregister_recv_cb(void)
        private extern int NativeEspNowUnregisterRecvCb();

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_register_send_cb(esp_now_send_cb_t cb)
        private extern int NativeEspNowRegisterSendCb(RegisterSendDelegate cb);

        [MethodImpl(MethodImplOptions.InternalCall)]
        // esp_err_t esp_now_unregister_send_cb(void)
        private extern int NativeEspNowUnregisterSendCb();

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
}
