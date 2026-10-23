# Düzeltilmesi Gerekenler – Yiğit'in Buldukları

23 Ekim 2026 · Yiğit Ahmet Hosta

Bu liste, `docs/Leaf_Brawl_SRS_Demo_v1.0.md` belgesinde değişmesi gereken 10 maddeyi toplar. Her madde, SRS'te şu an ne yazdığını, ne olması gerektiğini ve hangi maddeleri etkilediğini gösterir. Netleşmesi gereken noktalar en sondaki "Açık sorular" bölümündedir. SRS v1.1 bu listeye göre güncellenecek.

## Özet

| No | Konu | SRS'te etkilenen yerler |
| --- | --- | --- |
| 1 | Motor Unity olacak | 2.4 |
| 2 | Mermi yok, silah balta | 3.2, 6.1, FR-20, FR-21, FR-22, FR-54, BOT-06, BOT-09, UI-05, KT-03 |
| 3 | Ağaç kesme ve kesilen ağaçtan fazla yaprak | 3.3, 3.8, 4.4, FR-12, FR-50, yeni FR'ler |
| 4 | Çalılık: giren oyuncu görünmez olur | 3.8, 4.6, FR-51, FR-53, BOT-02 |
| 5 | Ölüm ganimetinin sade görünümü | 3.7, FR-81 |
| 6 | Hasar almayınca can yenilenmesi | 3.1, yeni FR |
| 7 | Bütün geliştirmeler en fazla 5. seviye | 3.1, 3.4, 3.6, FR-42 |
| 8 | 5. seviyede açılan tek kullanımlık yetenekler | 3.2, 6.1, 8, FR-22, UI-05, yeni FR'ler |
| 9 | Tarif maliyeti seviyeyle artar | 3.3, 3.4, FR-41 |
| 10 | Ana menüde oynanış kılavuzu | 2.3, 6.4, 8, FR-01, FR-02 |

## 1. Motor: Unity

- **SRS'te şu an (2.4):** "Motor ve dil ekip tercihidir (ör. Godot 4 veya Phaser 3)."
- **Olması gereken:** Motor **Unity**'dir, dil C#'tır. Oyun yine yerel ve çevrimdışı çalışan bir masaüstü yapısı olarak teslim edilir.

## 2. Mermi yok, silah balta

- **SRS'te şu an (3.2, FR-20, FR-21):** Temel saldırı sol tıkla atılan bir yaprak mermisidir (menzil 450 px).
- **Olması gereken:**
    - Oyunda **mermi kesinlikle yoktur**.
    - Her oyuncunun elinde silah olarak bir **balta** vardır. Balta yakın dövüş silahıdır.
    - Balta **fareye tıklayarak** veya klavyeden **L** tuşuna basarak sallanır.
    - Aynı balta hem rakiple savaşmak hem ağaç kesmek (madde 3) için kullanılır.
- **Etkilenenler:**
    - 3.2'deki "Yaprak fırlatma" satırı baltayla değiştirilir.
    - FR-20 ve FR-21 baltaya göre yeniden yazılır. FR-54 (mermiler taçtan geçer) kaldırılır.
    - BOT-06'daki "300–400 px mesafeyi koru" kuralı yakın dövüşe göre değişir. BOT-09'daki "tahmini konuma ateş" kuralı kalkar.
    - 6.1 kontrol tablosu ve UI-05 güncellenir. KT-03 baltaya göre yeniden yazılır.

## 3. Ağaç kesme

- **SRS'te şu an (3.3):** Ağaçlar yalnızca belirli aralıklarla kendiliğinden yaprak düşürür. Ağaç kesme yoktur.
- **Olması gereken:**
    - Ağaçların bir **canı (HP)** vardır ve balta ile kesilebilir.
    - Kesilen ağaç, kendiliğinden düşen yaprağı beklemekten **daha fazla** yaprak verir. Verilen yaprak sayısı ve türü rastgeledir.
    - Ağacın canı oyun dengesini bozmayacak şekilde ayarlanır. Ölçü: **en düşük güçteki oyuncu (Güç 1) bir ağacı yaklaşık 20 saniyede kesebilmelidir.**
- **Etkilenenler:** 3.3'e ağaç canı ve kesme ödülü eklenir. FR-12 (gövde engeli), FR-50 (taç alanı) ve 3.8'deki harita kesilen ağaç için güncellenir. 4.4'e yeni FR'ler eklenir.

## 4. Çalılık

- **SRS'te şu an (4.6, FR-51):** Ağaç taçlarının altındaki karakter, 160 px'ten uzaktaki karakterlere görünmez. Orta ve büyük yaprak yığını olanlar taç altında da görünür (FR-53).
- **Olması gereken:** Haritada **çalılıklar** bulunur. **Çalıya giren oyuncu görünmez olur.**
- **Etkilenenler:** 3.8'deki haritaya çalılıklar eklenir. 4.6, FR-51 ve FR-53 çalılığa göre yeniden yazılır. BOT-02'de botların çalıdaki oyuncuyu algılayamaması gerekir.

## 5. Ölüm ganimetinin görünümü

- **SRS'te şu an (3.7):** Ölen karakterin K, S, Y yapraklarının her birinin %35'i (aşağı yuvarlanır) ve T'nin %60'ı (yukarı yuvarlanır) 80 px çevreye **tek tek** saçılır.
- **Olması gereken:** Düşen yaprak miktarı ve oranları aynı kalır, ancak ekranda tek tek gösterilmez. Görüntü kirliliğini önlemek için ganimet yığın olarak gösterilir:

    | Düşen yaprak adedi | Ekranda görünen |
    | --- | --- |
    | 0–10 | 1 yaprak |
    | 10–20 | Üst üste 2 yaprak |
    | 20–30 | Üst üste 3 yaprak |

- Oyuncu yığının üstünden geçip topladığında **gerçek adetlerin tamamı** envantere eklenir; envanter listesinde ve arka planda gerçek sayılar görünür.
- **Etkilenenler:** 3.7 ve FR-81.

## 6. Can yenilenmesi

- **SRS'te şu an (3.1):** "Pasif can yenilenmesi yoktur."
- **Olması gereken:** Oyuncu **5 saniye boyunca hasar almazsa** canı yavaşça yenilenmeye başlar. Yenilenme hızı Sağlık seviyesine bağlıdır ve dengeli tutulur:

    | Sağlık seviyesi | Yenilenme hızı | Maksimum can |
    | --- | --- | --- |
    | 1 | 10 sn'de 1 HP | 100 |
    | 2 | 9 sn'de 1 HP | artar |
    | 3 | 8 sn'de 1 HP | artar |
    | 4 | 7 sn'de 1 HP | artar |
    | 5 | 6 sn'de 1 HP | artar |

- Hasar alındığında 5 saniyelik bekleme yeniden başlar.
- **Etkilenenler:** 3.1'e yenilenme satırı eklenir; 4.3'e yeni bir FR eklenir.

## 7. En fazla 5 seviye

- **SRS'te şu an (3.1):** Güç en fazla 6. seviyeye çıkar (5 tarif). Hız en fazla 4 tarifle, Sağlık en fazla 5 tarifle yükselir.
- **Olması gereken:** **Güç, Hız ve Sağlık geliştirmelerinin her biri en fazla 5. seviyeye çıkar.** Oyuncu her birinde 1. seviyeden başlar.
- **Etkilenenler:** 3.1 tablosu, 3.4 ve FR-42 ("En üst seviye" yazısı). 3.6'daki tehdit puanı kademeleri yeni üst sınıra göre gözden geçirilir.

## 8. 5. seviyede açılan yetenekler

- **SRS'te şu an:** Atılma (dash) maçın başından itibaren herkeste vardır (3.2, FR-22). Kalkan kapsam dışıdır (8). Ulti yoktur.
- **Olması gereken:** Bir geliştirme **5. seviyeye ulaşınca** ona özel bir yetenek açılır. Bu yetenekler maç boyunca **yalnızca bir kez** kullanılabilir.

    | Geliştirme | 5. seviyede açılan | Etkisi |
    | --- | --- | --- |
    | Sağlık | Kalkan | Clash Royale'deki Muhafız kalkanı ile Kıvılcım vuruşu arasındaki ilişki gibi: kalkan, büyük tek bir vuruşu tamamen emer. |
    | Hız | Atılma (dash) | **Atılma yalnızca Hız 5. seviyede açılır**; daha önce kullanılamaz. |
    | Güç | Ulti | Her şeyi tek vuruşta yok eden güçlü saldırı. |

- **Etkilenenler:** 3.2'den başlangıç atılması çıkarılır, FR-22 yeniden yazılır. 8'deki "kalkan kapsam dışı" satırı kaldırılır. 6.1 kontrollerine kalkan ve ulti tuşları, UI-05'e bu yeteneklerin göstergeleri eklenir. Bu yetenekler için yeni FR'ler yazılır.

## 9. Tarif maliyeti seviyeyle artar

- **SRS'te şu an (3.4):** Her tarifin maliyeti sabittir (ör. Güç: 4 K + 1 Y + 1 T).
- **Olması gereken:** Seviye yükseldikçe bir sonraki seviye için gereken yaprak sayısı artar. Güç için örnek:

    | Geçiş | Gereken yapraklar |
    | --- | --- |
    | 1 → 2 | 3 K + 1 S + 1 Y |
    | 2 → 3 | belirlenecek |
    | 3 → 4 | belirlenecek |
    | 4 → 5 | 25 K + 15 S + 15 Y |

- Hız ve Sağlık için de aynı mantıkla artan maliyet tablosu hazırlanır.
- **Etkilenenler:** 3.4 tablosu ve FR-41'deki tarif kartları. 3.3'teki envanter sınırı (K, S, Y için 15) 4 → 5 geçişinde gereken 25 K ile çelişir ve büyütülmelidir.

## 10. Ana menüde oynanış kılavuzu

- **SRS'te şu an (2.3, 8):** "Oyun içi eğitim bölümü yoktur"; ana menüde yalnızca tuşları gösteren "Kontroller" ekranı vardır.
- **Olması gereken:** Ana menüde oyunun **nasıl oynandığını anlatan bir kılavuz** bulunur: yaprak toplama, tarifler ve seviyeler, ağaç kesme, çalılık, can yenilenmesi, 5. seviye yetenekleri, daralan alan ve elenme.
- **Etkilenenler:** FR-01'deki menü seçenekleri, FR-02, 6.4 ve 8'deki "oyun içi eğitim bölümü yok" satırı.

## Açık sorular

Bu noktalar karar verilmeden SRS v1.1 yazılamaz.

1. **Ağaç kesme:** Kesilen ağaç geri büyür mü, büyürse ne kadar sürede? Kesilince kendiliğinden yaprak düşürmesi ve taç alanı ortadan kalkar mı? Köşe koruları da kesilebilir mi? Kesilen ağaç kaç yaprak verir (en az–en fazla), turuncu çıkma olasılığı nedir?
2. **Balta:** Menzili (px), vuruş bekleme süresi ve vuruş başına hasarı nedir? "Güç 1 ile ağaç ≈ 20 sn'de kesilir" kuralından ağaç canı = 20 sn ÷ bekleme süresi × Güç 1 hasarı olarak hesaplanabilir.
3. **Çalılık ve taç:** Çalılık ağaç taçlarından ayrı yeni bir öğe mi, taçların yerine mi geçiyor? Görünmezlik tam mı (yakına gelince de görünmez mi)? Büyük yaprak yığını olan güçlü oyuncu da çalıda görünmez olabilir mi?
4. **Ganimet yığını:** Yığın her yaprak türü için ayrı mı, yoksa tüm yapraklar tek yığında mı? 30'dan fazla yaprak düşerse kaç yaprak görünür? Sınırlar 10 ve 20'de çakışıyor; 1–10, 11–20, 21–30 olarak mı okunmalı?
5. **Can yenilenmesi:** Sağlık seviyesi başına maksimum can ne kadar artacak (ör. her seviyede +10 mu)?
6. **Kalkan:** Kalkanın canı veya süresi var mı? Ulti vuruşunu tamamen emip kırılıyor mu? Hangi tuşla açılıyor?
7. **Ulti:** "Her şeye tek atma" oyuncuları ve ağaçları kapsıyor mu? Etki alanı ve tuşu ne? Rakip kalkanlıysa ulti kalkanı kırıp durur mu?
8. **Tek kullanım:** Atılma da tek kullanımlık mı, yoksa Hız 5'te açılıp bekleme süresiyle tekrar kullanılabilir mi?
9. **Tarif maliyetleri:** Ara seviyelerin (2 → 3, 3 → 4) maliyetleri ve Hız ile Sağlık tabloları ne olacak? Turuncu yaprak (T) tariflerden tamamen mi çıkıyor, yoksa yalnızca örnekte mi yok? Çıkıyorsa turuncu yaprağın oyundaki rolü ne olacak?
