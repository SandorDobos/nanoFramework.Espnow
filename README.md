# nanoFramework.Hardware.Esp32.Espnow
📦 .NET nanoFramework class library for the ESP-NOW (connectionless Wi-Fi communication protocol) for ESP32 targets

## Example usage:

```csharp
var controller =  new EspNowController();
controller.DataReceived += (s, ea) =>
{
    Debug.WriteLine($"Received from {BitConverter.ToString(ea.PeerMac)} {ea.DataLen} bytes: {BitConverter.ToString(ea.Data)}");
}
controller.DataSent += (s, ea) =>
{
    Debug.WriteLine($"Status of sending to {BitConverter.ToString(ea.PeerMac)}: {ea.Status}");
}

controller.AddPeer(EspNowController.BROADCASTMAC, 0);
controller.Send(EspNowController.BROADCASTMAC, new byte[] { 1, 2, 3}, 3);
```
