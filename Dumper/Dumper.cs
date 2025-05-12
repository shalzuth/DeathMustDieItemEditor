using AssetsTools.NET.Extra;
using System.Text.Json;

namespace DeathMustDieItemEditor
{
    internal class Dumper
    {
        static string bundleFilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Death Must Die\Death Must Die_Data\StreamingAssets\aa\StandaloneWindows64";
        static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        public static void DumpData()
        {
            using var stream = File.OpenRead(bundleFilePath + @"\localization-assets-shared_assets_all.bundle");
            AssetsManager manager = new();
            var bunInst = manager.LoadBundleFile(stream, "fakeassets.assets");
            var fileInstance = manager.LoadAssetsFileFromBundle(bunInst, 0, false);
            var assetFile = fileInstance.file;
            var assetInfos = assetFile.AssetInfos.ToList();
            foreach (var mono in assetFile.GetAssetsOfType(AssetClassID.MonoBehaviour))
            {
                var monoBase = manager.GetBaseField(fileInstance, mono);
                var m_Name = monoBase["m_Name"].AsString;
                var m_TableCollectionName = monoBase["m_TableCollectionName"].AsString;
                if (m_Name == null) continue;
                var m_Entries = monoBase["m_Entries"]["Array"];
                var dict = new Dictionary<long, string> { };
                var swappeddict = new Dictionary<string, long> { };
                foreach(var entry in m_Entries.Children)
                {
                    var id = entry["m_Id"].AsLong;
                    var key = entry["m_Key"].AsString;
                    if (!dict.TryAdd(id, key))
                    {
                        Console.WriteLine("already added : " + id + ", " + key + " vs " + dict[id]);
                    }
                    swappeddict[key] = id;
                }
                File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Documents\GitHub\DeathMustDieItemEditor\WebApp\wwwroot\data\" + m_TableCollectionName + ".js", m_TableCollectionName + " = " + JsonSerializer.Serialize(swappeddict, jsonOptions));
            }
        }
        public static void DumpTranslations()
        {
            using var stream = File.OpenRead(bundleFilePath + @"\localization-string-tables-english(en)_assets_all.bundle");
            AssetsManager manager = new();
            var bunInst = manager.LoadBundleFile(stream, "fakeassets.assets");
            var fileInstance = manager.LoadAssetsFileFromBundle(bunInst, 0, false);
            var assetFile = fileInstance.file;
            var assetInfos = assetFile.AssetInfos.ToList();
            foreach (var mono in assetFile.GetAssetsOfType(AssetClassID.MonoBehaviour))
            {
                var monoBase = manager.GetBaseField(fileInstance, mono);
                var m_Name = monoBase["m_Name"].AsString;
                if (m_Name == null) continue;
                var m_Entries = monoBase["m_TableData"]["Array"];
                var dict = new Dictionary<long, string> { };
                foreach(var entry in m_Entries.Children)
                {
                    var id = entry["m_Id"].AsLong;
                    var key = entry["m_Localized"].AsString;
                    if (!dict.TryAdd(id, key))
                    {
                        Console.WriteLine("already added : " + id + ", " + key + " vs " + dict[id]);
                    }
                }
                File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Documents\GitHub\DeathMustDieItemEditor\WebApp\wwwroot\loc\en\" + m_Name.Replace("_en","") + ".js", m_Name + " = " + JsonSerializer.Serialize(dict, jsonOptions));
            }
        }
    }
}
