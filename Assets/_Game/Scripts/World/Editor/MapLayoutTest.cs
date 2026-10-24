using System.IO;
using LeafBrawl.Shared;
using UnityEditor;
using UnityEngine;

namespace LeafBrawl.World
{
    /// <summary>Menü: Leaf Brawl → Haritayı Test Et. 1000 tohumla MapLayoutGenerator'ı dener, özeti Console'a ve MapPreview/test_sonuc.txt'ye yazar.</summary>
    public static class MapLayoutTest
    {
        const int SeedCount = 1000;

        [MenuItem("Leaf Brawl/Haritayı Test Et")]
        static void Run()
        {
            MapGenSettings settings = MapGenSettings.FromConfig(Services.Config);

            int fallbacks = 0, maxAttempts = 0, wrongCounts = 0;
            long totalAttempts = 0;
            string firstWrong = null;

            for (int seed = 1; seed <= SeedCount; seed++)   // 0 = "saatten rastgele", o yüzden 1'den başla
            {
                MapLayout layout = MapLayoutGenerator.Generate(settings, new Rng(seed));

                if (layout.UsedFallback) fallbacks++;
                totalAttempts += layout.Attempts;
                if (layout.Attempts > maxAttempts) maxAttempts = layout.Attempts;

                int great = 0, trees = 0, rocks = 0, spawns = 0, bushes = 0;
                foreach (MapItem it in layout.Items)
                {
                    if (it.Kind == MapItemKind.GreatTree) great++;
                    else if (it.IsTree) trees++;
                    else if (it.Kind == MapItemKind.Rock) rocks++;
                    else if (it.Kind == MapItemKind.Spawn) spawns++;
                    else if (it.IsBush) bushes++;
                }

                if (great != 1 || trees != 44 || rocks != 4 || spawns != 4 || bushes != 12)
                {
                    wrongCounts++;
                    if (firstWrong == null)
                        firstWrong = $"tohum {seed}: ulu={great} ağaç={trees} kaya={rocks} doğma={spawns} çalılık={bushes}";
                }
            }

            string report =
                $"[Harita Testi] {SeedCount} tohum\n" +
                $"yedeğe düşen: {fallbacks}\n" +
                $"ortalama deneme: {(double)totalAttempts / SeedCount:F2}\n" +
                $"en çok deneme: {maxAttempts} (sınır {settings.MaxRetries})\n" +
                $"öğe sayısı hatalı tohum: {wrongCounts}" + (firstWrong != null ? " (ilk: " + firstWrong + ")" : " (hepsi 1 ulu / 44 ağaç / 4 kaya / 4 doğma / 12 çalılık)");

            Debug.Log(report);

            string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "MapPreview"));
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "test_sonuc.txt"), report);
        }
    }
}
