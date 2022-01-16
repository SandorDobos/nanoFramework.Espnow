//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Espnow
{
    /// <summary>
    /// Container for DataReceived event data.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// MAC address of peer data was received from.
        /// </summary>
        public byte[] PeerMac;

        /// <summary>
        /// Data received.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Length of received data.
        /// </summary>
        public int DataLen;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataReceivedEventArgs(byte[] peerMac, byte[] data, int dataLen)
        {
            this.PeerMac = peerMac;
            this.Data = data;
            this.DataLen = dataLen;
        }
    }
}
