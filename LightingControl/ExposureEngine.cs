// Desc:  Decide how much to brighten the screen.

//This is where the “reactive” part lives.
//Responsibilities
//• 	Map luminance → target gamma
//• 	Smooth transitions over time
//• 	Apply caps (e.g., never brighten more than 20%)
//• 	Handle hysteresis (avoid flicker when brightness hovers around a threshold)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingControl
{
    internal class ExposureEngine
    {
        currentGamma = Lerp(currentGamma, targetGamma, 0.1f);

        if (stats.shadow10< 0.15)
    targetGamma = 0.85; // brighten
else if (stats.average< 0.25)
    targetGamma = 0.90;
else
    targetGamma = 1.0; // no change
    }
    struct ExposureDecision
    {
        float gamma; // 0.8 = brighter, 1.0 = neutral
    }
}
