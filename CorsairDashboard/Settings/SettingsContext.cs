using CorsairDashboard.Common.SqliteMigrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CorsairDashboard.Settings
{
    [Export]
    public partial class SettingsContext : DbContext, ISettings
    {
        private const string AccentColorKey = "Theme.AccentColor";
        private const string ThemeColorKey = "Theme.Color";
        private const string MaxRpmDeltaKey = "Fans.MaxRpmDelta";

        public DbSet<KeyValueSetting> KeyValueSettings { get; set; }

        public DbSet<DeviceSettings> DevicesSettings { get; set; }

        public DbSet<FanSettings> FansSettings { get; set; }

        public String AccentColor
        {
            get
            {
                return GetKeyValueSetting(AccentColorKey);
            }
            set
            {
                SetKeyValueSetting(AccentColorKey, value);
            }
        }

        public String ThemeColor
        {
            get
            {
                return GetKeyValueSetting(ThemeColorKey);
            }
            set
            {
                SetKeyValueSetting(ThemeColorKey, value);
            }
        }

        public UInt16 MaxRpmDelta
        {
            get
            {
                UInt16 maxRpmDelta;
                UInt16.TryParse(GetKeyValueSetting(MaxRpmDeltaKey), out maxRpmDelta);
                return maxRpmDelta;
            }
            set
            {
                SetKeyValueSetting(MaxRpmDeltaKey, value.ToString());
            }
        }

        public string GetLabelForFan(Guid deviceId, int fanNr)
        {
            var fanSettings = FansSettings.Find(deviceId, fanNr);
            return fanSettings != null ? fanSettings.Label : String.Empty;
        }

        public void SetLabelForFan(Guid deviceId, int fanNr, String label)
        {
            var deviceSettings = GetOrCreateDevice(deviceId);
            var fanSettings = deviceSettings.FansSettings.FirstOrDefault(f => f.FanNr == fanNr);
            if (fanSettings == null)
            {
                fanSettings = new FanSettings()
                {
                    DeviceId = deviceId,
                    FanNr = fanNr
                };
                deviceSettings.FansSettings.Add(fanSettings);
            }
            fanSettings.Label = label;
        }

        private DeviceSettings GetOrCreateDevice(Guid deviceId)
        {
            var deviceSettings = DevicesSettings.Find(deviceId);
            if (deviceSettings == null)
            {
                deviceSettings = new DeviceSettings()
                {
                    DeviceId = deviceId,
                    FansSettings = new List<FanSettings>()
                };
                DevicesSettings.Add(deviceSettings);
            }
            return deviceSettings;
        }

        public void Initialize()
        {
            if (!KeyValueSettings.Any())
            {
                ResetToDefaults();
            }
        }

        public void ResetToDefaults()
        {
            AccentColor = "Blue";
            ThemeColor = "BaseDark";
            MaxRpmDelta = 150;
        }

        public void SaveSettings()
        {
            SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<KeyValueSetting>().HasKey(kv => kv.Key).ToTable("KeyValueSettings");
            modelBuilder.Entity<DeviceSettings>().HasKey(d => d.DeviceId).ToTable("DevicesSettings");
            modelBuilder.Entity<FanSettings>().HasKey(fs => new { fs.DeviceId, fs.FanNr }).ToTable("FansSettings");
        }

        private String GetKeyValueSetting(String key)
        {
            return KeyValueSettings.Where(kv => kv.Key == key)
                .Select(kv => kv.Value)
                .FirstOrDefault();
        }

        private void SetKeyValueSetting(String key, String value)
        {
            var kvs = KeyValueSettings.FirstOrDefault(kv => kv.Key == key);
            if (kvs == null)
            {
                kvs = new KeyValueSetting() { Key = key, Value = value };
                KeyValueSettings.Add(kvs);
            }
            else
            {
                kvs.Value = value;
            }
            SaveChanges();
        }
    }

    #region Migrations

    public partial class SettingsContext : ISqliteMigrationsSupport
    {
        public IEnumerable<Migration> Migrations
        {
            get { return _Migrations; }
        }

        static List<Migration> _Migrations = new List<Migration>()
        {
            new InitialMigration()
        };

        class InitialMigration : Migration
        {
            public override uint Version
            {
                get { return 1; }
            }

            public InitialMigration()
            {
                AddStatement("CREATE TABLE KeyValueSettings (Key TEXT PRIMARY KEY, Value TEXT)");
                AddStatement("CREATE TABLE DevicesSettings (DeviceId BLOB PRIMARY KEY)");
                AddStatement("CREATE TABLE FansSettings (DeviceId BLOB, FanNr INTEGER, Label TEXT, PRIMARY KEY (DeviceId, FanNr))");
            }
        }
    }

    #endregion
}
