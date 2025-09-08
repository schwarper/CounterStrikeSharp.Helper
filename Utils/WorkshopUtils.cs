using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.Helper.Utils;

/// <summary>
/// Provides utility methods for retrieving information related to the Steam Workshop,
/// specifically the currently loaded workshop map ID.
/// </summary>
/// <remarks>Credits to rasco for the original implementation.</remarks>
public static class WorkshopUtils
{
    private static readonly nint _networkServerService;
    private static readonly object _lock = new();

    private delegate nint GetGameServerHandle(nint networkServerService);
    private static GetGameServerHandle? _getGameServerHandleDelegate;

    private delegate nint GetWorkshopId(nint gameServer);
    private static GetWorkshopId? _getWorkshopIdDelegate;

    static WorkshopUtils()
    {
        _networkServerService = NativeAPI.GetValveInterface(0, "NetworkServerService_001");
    }

    private static void EnsureDelegatesInitialized()
    {
        if (_getGameServerHandleDelegate != null && _getWorkshopIdDelegate != null)
            return;

        lock (_lock)
        {
            if (_getGameServerHandleDelegate == null)
            {
                unsafe
                {
                    int gameServerOffset = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 24 : 23;
                    nint* gameServerHandlePtr = *(nint**)_networkServerService + gameServerOffset;
                    _getGameServerHandleDelegate = Marshal.GetDelegateForFunctionPointer<GetGameServerHandle>(*gameServerHandlePtr);
                }
            }

            if (_getWorkshopIdDelegate == null)
            {
                unsafe
                {
                    nint networkGameServer = _getGameServerHandleDelegate!(_networkServerService);
                    nint* workshopHandlePtr = *(nint**)networkGameServer + 25;
                    _getWorkshopIdDelegate = Marshal.GetDelegateForFunctionPointer<GetWorkshopId>(*workshopHandlePtr);
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the ID of the currently loaded Steam Workshop map.
    /// </summary>
    /// <returns>A string representing the workshop ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the workshop ID cannot be retrieved.</exception>
    public static string GetID()
    {
        EnsureDelegatesInitialized();

        unsafe
        {
            nint networkGameServer = _getGameServerHandleDelegate!(_networkServerService);
            nint workshopPtr = _getWorkshopIdDelegate!(networkGameServer);
            string? workshopString = Marshal.PtrToStringAnsi(workshopPtr);
            return workshopString?.Split(',')[0] ?? throw new InvalidOperationException("Failed to retrieve the workshop ID.");
        }
    }
}