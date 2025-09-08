using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;

namespace CounterStrikeSharp.Helper.Utils;

/// <summary>
/// Provides utility methods for retrieving server-related information,
/// such as the public IP address and port.
/// </summary>
/// <remarks>Credits to nuko for the original implementation.</remarks>
public static class ServerUtils
{
    private delegate nint CNetworkSystem_UpdatePublicIp(nint a1);
    private static CNetworkSystem_UpdatePublicIp? _networkSystemUpdatePublicIp;
    private static readonly nint _networkSystem;
    private static readonly int _port;
    private static readonly object _lock = new();

    static ServerUtils()
    {
        _networkSystem = NativeAPI.GetValveInterface(0, "NetworkSystemVersion001");
        _port = ConVar.Find("hostport")?.GetPrimitiveValue<int>() ?? 0;
    }

    /// <summary>
    /// Gets the public IP address of the server.
    /// </summary>
    /// <returns>A string representing the server's public IP address in dot-separated format.</returns>
    public static string GetServerIP()
    {
        EnsureDelegateInitialized();

        unsafe
        {
            byte* ipBytes = (byte*)(_networkSystemUpdatePublicIp!(_networkSystem) + 4);
            return string.Join('.', ipBytes[0], ipBytes[1], ipBytes[2], ipBytes[3]);
        }
    }

    /// <summary>
    /// Gets the port of the server.
    /// </summary>
    /// <returns>An integer representing the server's host port.</returns>
    public static int GetServerPort()
    {
        return _port;
    }

    private static void EnsureDelegateInitialized()
    {
        if (_networkSystemUpdatePublicIp != null)
            return;

        lock (_lock)
        {
            if (_networkSystemUpdatePublicIp == null)
            {
                unsafe
                {
                    nint funcPtr = *(nint*)(*(nint*)_networkSystem + 256);
                    _networkSystemUpdatePublicIp = Marshal.GetDelegateForFunctionPointer<CNetworkSystem_UpdatePublicIp>(funcPtr);
                }
            }
        }
    }
}