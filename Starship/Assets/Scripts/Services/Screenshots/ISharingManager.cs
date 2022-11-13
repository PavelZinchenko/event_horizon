using UnityEngine;

namespace Services.Screenshots
{
    public interface ISharingManager
    {
        void ShareScreenshot(Texture2D screenshot, string filename, string pcDirectory);
    }
}
