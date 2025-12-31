using System;
using System.Runtime.InteropServices;
using Windows.Graphics.DirectX.Direct3D11;

public static class Direct3D11Helper
{
    [DllImport("d3d11.dll", EntryPoint = "D3D11CreateDevice")]
    private static extern int D3D11CreateDevice(
        IntPtr adapter,
        int driverType,
        IntPtr software,
        uint flags,
        IntPtr featureLevels,
        int featureLevelsCount,
        uint sdkVersion,
        out IntPtr device,
        out int featureLevel,
        out IntPtr immediateContext);

    public static IDirect3DDevice CreateDevice()
    {
        IntPtr devicePtr;
        IntPtr contextPtr;
        int featureLevel;

        // Create a basic D3D11 device
        D3D11CreateDevice(
            IntPtr.Zero,
            1, // D3D_DRIVER_TYPE_HARDWARE
            IntPtr.Zero,
            0,
            IntPtr.Zero,
            0,
            7, // D3D11_SDK_VERSION
            out devicePtr,
            out featureLevel,
            out contextPtr);

        // Wrap it in a WinRT-friendly interface
        return CreateDirect3DDevice(devicePtr);
    }

    private static IDirect3DDevice CreateDirect3DDevice(IntPtr devicePtr)
    {
        var dxgiDevice = (IDXGIDevice)Marshal.GetObjectForIUnknown(devicePtr);
        return Direct3D11Helper.CreateDirect3DDevice(dxgiDevice);
    }
}

[ComImport, Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IDXGIDevice { }