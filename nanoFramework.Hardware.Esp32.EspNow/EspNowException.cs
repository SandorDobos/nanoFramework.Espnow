using System;

namespace nanoFramework.Hardware.Esp32.EspNow
{
    /// <summary>
    /// EspNow related exception
    /// </summary>
    public class EspNowException : Exception
    {
        /// <summary>
        /// esp_err_t values, see esp_now.h, esp_wifi.h, etc.
        /// </summary>
        public int esp_err;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="esp_err">esp_err_t values, see esp_now.h, esp_wifi.h, etc.</param>
        public EspNowException(int esp_err)
            : base(esp_err.ToString())
        {
            this.esp_err = esp_err;
        }
    }
}
