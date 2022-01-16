//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Espnow
{
    /// <summary>
    /// Container for DataSent event data.
    /// </summary>
    public class DataSentEventArgs : EventArgs
    {
        /// <summary>
        /// MAC address of peer data was sent to.
        /// </summary>
        public byte[] PeerMac;

        /// <summary>
        /// Status of sending.
        /// See esp_now_send_status_t in esp_now.h
        /// </summary>
        public int Status;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataSentEventArgs(byte[] peerMac, int status)
        {
            this.PeerMac = peerMac;
            this.Status = status;
        }
    }
}
