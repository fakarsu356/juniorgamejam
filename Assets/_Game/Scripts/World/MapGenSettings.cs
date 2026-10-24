using LeafBrawl.Shared;

namespace LeafBrawl.World
{
    /// <summary>
    /// Harita üretiminin girdileri (SRS 3.12). Uzunluklar dünya px'tir.
    /// Merkez değerler balance.json "map" bölümünden gelir (<see cref="FromConfig"/>).
    /// Alttaki "sapma" alanları henüz balance.json'da yok: "sözleşme" PR'ı ile Balance.MapConfig'e
    /// eklenince FromConfig oradan okuyacak ve buradaki varsayılanlar silinecek.
    /// </summary>
    public class MapGenSettings
    {
        // ---- balance.json "map" ----
        public float MapSizePx, ClearingRadiusPx;
        public float GreatTreeTrunkRadiusPx;
        public float RockRingPx, RockSizePx;
        public float[] RockAnglesDeg;
        public int ClusterCount;
        public float ClusterRingPx, ClusterStartDeg, ClusterStepDeg, ClusterRadiusPx;
        public int ClusterTreeCount;
        public float ClusterTreeOffsetPx, ClusterTreeStepDeg;
        public float GroveRingPx, GroveRadiusPx;
        public float[] GroveAnglesDeg;
        public int GroveCenterTrees, GroveRingTrees;
        public float GroveRingOffsetPx, GroveRingStepDeg;
        public float TreeTrunkRadiusPx;
        public int OuterBushCount;
        public float OuterBushRingPx, OuterBushStartDeg, OuterBushStepDeg, OuterBushRadiusPx;
        public float InnerBushRingPx, InnerBushRadiusPx;
        public float[] InnerBushAnglesDeg;
        public float SpawnRingPx;
        public float[] SpawnAnglesDeg;

        /// <summary>balance.json "zone": ilk daralmanın bittiği yarıçap. Koru ağaçları bunun dışında kalmalı (SRS 3.12). 0 = kontrol yok.</summary>
        public float GroveOutsideZonePx;

        // ---- Sapma ve doğrulama [geçici: balance.json "map"e sözleşme PR'ı ile taşınacak] ----
        public float AngleJitterDeg = 6f;        // küme ve koru merkezi
        public float RadiusJitterPx = 50f;       // küme ve koru merkezi
        public float RockAngleJitterDeg = 12f;
        public float RockRadiusJitterPx = 30f;
        public float BushAngleJitterDeg = 6f;
        public float BushRadiusJitterPx = 25f;
        public float TreeJitterPx = 10f;         // küme/koru içindeki her ağaç (x ve y ayrı)
        public float BoundsMarginPx = 40f;       // harita kenarından en az bu kadar içeride
        public float MinGapPx = 20f;             // gövde ve kaya arası en az boşluk
        public float MinPassagePx = 80f;         // küme/koru daireleri arası en az geçit
        public float SpawnClearPx = 150f;        // doğma noktası çevresi boş alan
        public int MaxRetries = 50;

        public static MapGenSettings FromConfig(Balance b)
        {
            MapConfig m = b.map;
            return new MapGenSettings
            {
                MapSizePx = m.sizePx,
                ClearingRadiusPx = m.clearingRadiusPx,
                GreatTreeTrunkRadiusPx = m.greatTreeTrunkRadiusPx,
                RockRingPx = m.rockRingPx,
                RockSizePx = m.rockSizePx,
                RockAnglesDeg = m.rockAnglesDeg,
                ClusterCount = m.clusterCount,
                ClusterRingPx = m.clusterRingPx,
                ClusterStartDeg = m.clusterStartDeg,
                ClusterStepDeg = m.clusterStepDeg,
                ClusterRadiusPx = m.clusterRadiusPx,
                ClusterTreeCount = m.clusterTreeCount,
                ClusterTreeOffsetPx = m.clusterTreeOffsetPx,
                ClusterTreeStepDeg = m.clusterTreeStepDeg,
                GroveRingPx = m.groveRingPx,
                GroveRadiusPx = m.groveRadiusPx,
                GroveAnglesDeg = m.groveAnglesDeg,
                GroveCenterTrees = m.groveCenterTrees,
                GroveRingTrees = m.groveRingTrees,
                GroveRingOffsetPx = m.groveRingOffsetPx,
                GroveRingStepDeg = m.groveRingStepDeg,
                TreeTrunkRadiusPx = m.treeTrunkRadiusPx,
                OuterBushCount = m.outerBushCount,
                OuterBushRingPx = m.outerBushRingPx,
                OuterBushStartDeg = m.outerBushStartDeg,
                OuterBushStepDeg = m.outerBushStepDeg,
                OuterBushRadiusPx = m.outerBushRadiusPx,
                InnerBushRingPx = m.innerBushRingPx,
                InnerBushAnglesDeg = m.innerBushAnglesDeg,
                InnerBushRadiusPx = m.innerBushRadiusPx,
                SpawnRingPx = m.spawnRingPx,
                SpawnAnglesDeg = m.spawnAnglesDeg,
                GroveOutsideZonePx = FirstShrinkEndRadius(b.zone),
            };
        }

        // İlk gerçekten daralan aşamanın bitiş yarıçapı (3.8: 1700 → 1000).
        static float FirstShrinkEndRadius(ZoneConfig zone)
        {
            if (zone == null || zone.phases == null) return 0f;
            foreach (ZonePhase p in zone.phases)
                if (p.endRadiusPx < p.startRadiusPx) return p.endRadiusPx;
            return 0f;
        }
    }
}
