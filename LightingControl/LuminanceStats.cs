// Desc: Convert the frame into a single “how bright is this scene?” value.
//Responsibilities
//- Compute average luminance
//- Optionally compute:
//-Median luminance
//- Shadow percentile(e.g., 10th percentile)
//- Highlight percentile


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingControl
{
    struct LuminanceStats
    {
        float average;
        float median;
        float shadow10;
        float highlight90;
    }
}
