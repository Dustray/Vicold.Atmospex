using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMIAS.VISION.Tool
{
    internal class FPSTool
    {
        private int frameCount = 0;

        private uint lastMsec = 0;

        public Action<int> OnFPSUpdate;

        public void Update(uint msec)
        {
            if (msec - lastMsec > 500)
            {
                lastMsec = msec;
                OnFPSUpdate.Invoke(frameCount*2);
                frameCount = 0;
            }

            frameCount++;
        }

    }
}
