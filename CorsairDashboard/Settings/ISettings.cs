using System;
using System.Threading.Tasks;

namespace CorsairDashboard.Settings
{
    public interface ISettings
    {
        String AccentColor { get; set; }

        String ThemeColor { get; set; }
        
        UInt16 MaxRpmDelta { get; set; }

        String GetLabelForFan(Guid deviceId, int fanNr);

        void SetLabelForFan(Guid deviceId, int fanNr, String label);

        void Initialize();

        void ResetToDefaults();

        void SaveSettings();
    }
}