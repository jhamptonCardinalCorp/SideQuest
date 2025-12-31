// Desc: Get a tiny snapshot of the screen without tanking performance.

//Responsibilities
//- Capture the screen at something like 64×36 or 128×72
//- Downscale immediately
//- Provide a simple byte[] or Color[] buffer to the analyzer

// Notes

//- Use Windows.Graphics.Capture(fast, modern, works in windowed and borderless)
//- Capture every 100–200 ms
//- You don’t need full fidelity — just enough to estimate brightnes


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

namespace LightingControl
{
    struct FrameData
    {
        int width;
        int height;
        byte[] luminanceValues; // or RGB if you prefer
    }
}
