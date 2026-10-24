using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LeafBrawl.Core;
using LeafBrawl.Shared;

namespace LeafBrawl.Presentation
{
    /// <summary>
    /// MainMenu.unity sahnesindeki Ana Menü (FR-01), Kontroller (FR-02) ve
    /// 6 sayfalık Kılavuz (FR-03, 6.4, NFR-03) ekranlarını yönetir.
    /// Sahne boşsa tüm kamera, EventSystem ve UI panellerini anında otomatik kurar.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("İsteğe Bağlı Görseller (Boş bırakılırsa otomatik üretilir)")]
        [SerializeField] private Sprite logoSprite;
        [SerializeField] private Sprite[] guidePageSprites = new Sprite[6];

        [Header("İsteğe Bağlı Özel Font")]
        [SerializeField] private Font customFont;

        private GameObject _mainPanel;
        private GameObject _controlsPanel;
        private GameObject _guidePanel;

        // Kılavuz (FR-03) Bileşenleri
        private int _currentGuidePage;
        private Text _guideTitleText;
        private Text _guideBodyText;
        private Text _guidePageIndicatorText;
        private Image _guideIllustrationImage;
        private Button _prevPageButton;
        private Button _nextPageButton;

        private struct GuidePageData
        {
            public string Title;
            public string Body; // En fazla 3 cümle (SRS v1.4 Bölüm 3)
            public Color ThemeColor;
        }

        // SRS v1.4 Bölüm 3'ten sade dille yazılmış 6 sayfa (Her biri tam 3 cümle, 10 sn alan uyarısı yok)
        private static readonly GuidePageData[] GuidePages = new GuidePageData[]
        {
            new GuidePageData
            {
                Title = "1. Yaprak Toplama ve Ağaç Kesme",
                Body = "Ağaçlardan belirli aralıklarla Kırmızı (K), Sarı (S), Yeşil (Y) ve Turuncu (T) yapraklar düşer; yaprağa yaklaştığında otomatik toplanır.\n" +
                       "Merkezdeki ulu ağaç kesilemez ancak en sık ve en yüksek turuncu olasılığıyla yaprak üretir.\n" +
                       "Küme ve koru ağaçlarını baltayla keserek 6–10 adet ekstra yaprak elde edebilirsin; kesilen ağacın yerinde engel olmayan kütük kalır.",
                ThemeColor = new Color(0.85f, 0.42f, 0.20f)
            },
            new GuidePageData
            {
                Title = "2. Tarifler, Seviyeler ve \"Güvende\" Kuralı",
                Body = "Topladığın yapraklarla 1, 2, 3 tuşlarına basarak Güç, Hız ve Sağlık geliştirmelerini en fazla 5. seviyeye kadar yükseltebilirsin.\n" +
                       "Tarif üretimi yalnızca karakter \"güvendeyken\" (güvenli alanın içindeyken ve son 3 sn içinde hasar almamış, balta savurmamışken) başlatılabilir.\n" +
                       "Üretim 1,5 sn sürer; bu sürede hareket edebilirsin ancak hasar alırsan üretim iptal olur ve yaprakların harcanmaz.",
                ThemeColor = new Color(0.90f, 0.72f, 0.20f)
            },
            new GuidePageData
            {
                Title = "3. Can Yenilenmesi ve Can İksiri",
                Body = "Karakter 5 sn boyunca hasar almazsa canı, mevcut Sağlık seviyesine göre kendiliğinden yavaşça yenilenmeye başlar.\n" +
                       "Savaşta hızlı iyileşmek için 2 Yeşil (Y) ve 1 Turuncu (T) yaprak harcayarak 4 tuşuyla Can İksiri üretebilirsin.\n" +
                       "Can iksiri üretildiği anda içilir, mevcut canı 40 artırır ve tehdit puanını yükseltmez.",
                ThemeColor = new Color(0.28f, 0.80f, 0.32f)
            },
            new GuidePageData
            {
                Title = "4. 5. Seviye Yetenekleri",
                Body = "Maç başında özel yetenekler kapalıdır; bir geliştirme 5. seviyeye ulaştığında ona özel yetenek açılır.\n" +
                       "Hız 5'te Boşluk tuşuyla tekrar kullanılabilir Atılma (Dash); Sağlık 5'te Q tuşuyla maçta 1 kez ilk vuruşu emen Kalkan açılır.\n" +
                       "Güç 5'te E tuşuyla maçta 1 kez kullanılan Ulti açılır; kısa hazırlıktan sonra çevrendeki rakipleri eler ve kesilebilir ağaçları keser.",
                ThemeColor = new Color(0.35f, 0.75f, 0.95f)
            },
            new GuidePageData
            {
                Title = "5. Çalılıklar ve Yaprak Yığını",
                Body = "Haritadaki çalılıkların içine giren karakter ve yaprak yığını, aynı çalılıkta olmayan bütün karakterlere görünmez olur.\n" +
                       "Çalılıktaki karakter balta savurduğunda veya hasar aldığında 1 sn boyunca herkese görünür hale gelir.\n" +
                       "Geliştirme seviyelerin arttıkça tehdit puanın yükselir ve karakterinin etrafındaki yaprak yığını büyür.",
                ThemeColor = new Color(0.72f, 0.28f, 0.18f)
            },
            new GuidePageData
            {
                Title = "6. Daralan Alan ve Elenme",
                Body = "Güvenli alan harita merkezinde sabit bir dairedir ve maç ilerledikçe merkeze doğru daralır.\n" +
                       "Alan dışındaki karakterler her 0,5 sn'de bir hasar alır ve gövdesi alan dışında kalan ağaçlar yaprak üretmeyi bırakır.\n" +
                       "Yeniden doğma yoktur; ölen karakter ganimet yığınını bırakarak elenir ve hayatta kalan son karakter maçı kazanır.",
                ThemeColor = new Color(0.60f, 0.35f, 0.75f)
            }
        };

        private Sprite[] _proceduralGuideSprites;

        private void Awake()
        {
            EnsureCameraAndEventSystem();
            GenerateFallbackSpritesIfNeeded();
            BuildAllScreens();
            ShowMainMenu();
        }

        private void Update()
        {
            // SRS 6.4: Kontroller ve Kılavuz ekranlarında Esc ana menüye döner
            if (WasEscapePressedThisFrame())
            {
                if ((_controlsPanel != null && _controlsPanel.activeSelf) ||
                    (_guidePanel != null && _guidePanel.activeSelf))
                {
                    ShowMainMenu();
                }
            }
        }

        private static bool WasEscapePressedThisFrame()
        {
            // Yeni Input System (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            try
            {
                Type kbType = Type.GetType("UnityEngine.InputSystem.Keyboard, Unity.InputSystem");
                if (kbType != null)
                {
                    object currentKb = kbType.GetProperty("current", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                    if (currentKb != null)
                    {
                        object escKey = kbType.GetProperty("escapeKey", BindingFlags.Public | BindingFlags.Instance)?.GetValue(currentKb);
                        if (escKey != null)
                        {
                            PropertyInfo pressedProp = escKey.GetType().GetProperty("wasPressedThisFrame", BindingFlags.Public | BindingFlags.Instance);
                            if (pressedProp != null && (bool)pressedProp.GetValue(escKey))
                                return true;
                        }
                    }
                }
            }
            catch { }

            // Eski Input Manager yedeği
            try
            {
                return Input.GetKeyDown(KeyCode.Escape);
            }
            catch
            {
                return false;
            }
        }

        // --- DÜĞME EYLEMLERİ (Yalnızca izin verilen SceneFlow çağrıları) ---
        private void OnStartClicked()
        {
            SceneFlow.StartMatch();
        }

        private void OnGuideClicked()
        {
            _mainPanel.SetActive(false);
            _controlsPanel.SetActive(false);
            _guidePanel.SetActive(true);
            SetGuidePage(0);
        }

        private void OnControlsClicked()
        {
            _mainPanel.SetActive(false);
            _guidePanel.SetActive(false);
            _controlsPanel.SetActive(true);
        }

        private void OnQuitClicked()
        {
            SceneFlow.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void ShowMainMenu()
        {
            if (_mainPanel != null) _mainPanel.SetActive(true);
            if (_controlsPanel != null) _controlsPanel.SetActive(false);
            if (_guidePanel != null) _guidePanel.SetActive(false);
        }

        private void ChangeGuidePage(int delta)
        {
            SetGuidePage(Mathf.Clamp(_currentGuidePage + delta, 0, GuidePages.Length - 1));
        }

        private void SetGuidePage(int index)
        {
            _currentGuidePage = Mathf.Clamp(index, 0, GuidePages.Length - 1);
            GuidePageData page = GuidePages[_currentGuidePage];

            if (_guideTitleText != null) _guideTitleText.text = page.Title;
            if (_guideBodyText != null) _guideBodyText.text = page.Body;
            if (_guidePageIndicatorText != null) _guidePageIndicatorText.text = $"Sayfa {_currentGuidePage + 1} / {GuidePages.Length}";

            if (_guideIllustrationImage != null)
            {
                Sprite custom = (guidePageSprites != null && _currentGuidePage < guidePageSprites.Length)
                    ? guidePageSprites[_currentGuidePage]
                    : null;
                _guideIllustrationImage.sprite = custom != null ? custom : _proceduralGuideSprites[_currentGuidePage];
                _guideIllustrationImage.color = Color.white;
            }

            if (_prevPageButton != null) _prevPageButton.interactable = _currentGuidePage > 0;
            if (_nextPageButton != null) _nextPageButton.interactable = _currentGuidePage < GuidePages.Length - 1;
        }

        // --- SAHNE KAMERA VE EVENTSYSTEM KURULUMU ---
        private static void EnsureCameraAndEventSystem()
        {
            if (Camera.main == null && FindAnyObjectByType<Camera>() == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                Camera cam = camObj.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.12f, 0.08f, 0.06f, 1f);
                cam.orthographic = true;
            }

            if (FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();

                Type newInputModule = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (newInputModule != null)
                    esObj.AddComponent(newInputModule);
                else
                    esObj.AddComponent<StandaloneInputModule>();
            }
        }

        // --- ARAYÜZ İNŞASI (640x360 Referans Çözünürlük, Tek Sütun 4 Düğme - SRS 6.4) ---
        private void BuildAllScreens()
        {
            Font font = customFont != null ? customFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject canvasObj = new GameObject("MainMenu_Canvas");
            canvasObj.transform.SetParent(transform, false);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(640f, 360f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // Tam ekran sonbahar arka planı
            CreatePanel(canvasObj.transform, "Background", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.13f, 0.09f, 0.07f, 1f));

            // 1. ANA MENÜ PANELİ (FR-01: Logo + Tek Sütunda 4 Düğme)
            _mainPanel = CreatePanel(canvasObj.transform, "Panel_MainMenu", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(260f, 290f), new Color(0.20f, 0.13f, 0.09f, 0.96f));

            if (logoSprite != null)
            {
                GameObject logoObj = CreatePanel(_mainPanel.transform, "LogoImage", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -42f), new Vector2(180f, 46f), Color.white);
                logoObj.GetComponent<Image>().sprite = logoSprite;
            }
            else
            {
                Text logoTxt = CreateText(_mainPanel.transform, "LogoText", "🍁 LEAF BRAWL", 22, new Vector2(0f, 102f), new Vector2(240f, 44f), font, TextAnchor.MiddleCenter);
                logoTxt.color = new Color(1f, 0.72f, 0.25f);
            }

            CreateMenuButton(_mainPanel.transform, "Btn_Start",    "Başla",      new Vector2(0f, 38f),  new Vector2(180f, 34f), font, OnStartClicked);
            CreateMenuButton(_mainPanel.transform, "Btn_Guide",    "Kılavuz",    new Vector2(0f, -6f),  new Vector2(180f, 34f), font, OnGuideClicked);
            CreateMenuButton(_mainPanel.transform, "Btn_Controls", "Kontroller", new Vector2(0f, -50f), new Vector2(180f, 34f), font, OnControlsClicked);
            CreateMenuButton(_mainPanel.transform, "Btn_Quit",     "Çıkış",      new Vector2(0f, -94f), new Vector2(180f, 34f), font, OnQuitClicked);

            // 2. KONTROLLER EKRANI (FR-02: SRS 6.1'deki tüm tuşlar tek sayfada)
            _controlsPanel = CreatePanel(canvasObj.transform, "Panel_Controls", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460f, 310f), new Color(0.20f, 0.13f, 0.09f, 0.98f));

            Text ctrlTitle = CreateText(_controlsPanel.transform, "Title", "KONTROLLER", 16, new Vector2(0f, 128f), new Vector2(420f, 28f), font, TextAnchor.MiddleCenter);
            ctrlTitle.color = new Color(1f, 0.82f, 0.35f);

            string controlsList =
                "<b>W / A / S / D</b>   —   8 yönde hareket\n" +
                "<b>Fare</b>   —   Bakış ve vuruş yönü\n" +
                "<b>Sol Tık veya L</b>   —   Balta savurma (basılı tut: sürekli)\n" +
                "<b>Boşluk</b>   —   Atılma / Dash (Hız 5'te açılır)\n" +
                "<b>Q</b>   —   Kalkan (Sağlık 5'te açılır, maçta 1 kez)\n" +
                "<b>E</b>   —   Ulti (Güç 5'te açılır, maçta 1 kez)\n" +
                "<b>1 / 2 / 3</b>   —   Güç / Hız / Sağlık geliştirmesini üret\n" +
                "<b>4</b>   —   Can iksiri üret (+40 Can)\n" +
                "<b>Tab</b>   —   Envanter panelini aç / kapat\n" +
                "<b>Esc</b>   —   Maçı duraklat / Ana menüye dön";

            Text ctrlBody = CreateText(_controlsPanel.transform, "ControlsTable", controlsList, 11, new Vector2(0f, 6f), new Vector2(410f, 195f), font, TextAnchor.MiddleLeft);
            ctrlBody.lineSpacing = 1.25f;

            CreateMenuButton(_controlsPanel.transform, "Btn_BackFromControls", "Ana Menü [Esc]", new Vector2(0f, -122f), new Vector2(160f, 30f), font, ShowMainMenu);

            // 3. KILAVUZ EKRANI (FR-03: İleri/Geri ile 6 Sayfa, Başlık + <=3 Cümle + Görsel)
            _guidePanel = CreatePanel(canvasObj.transform, "Panel_Guide", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(480f, 315f), new Color(0.20f, 0.13f, 0.09f, 0.98f));

            _guideTitleText = CreateText(_guidePanel.transform, "GuideTitle", "", 15, new Vector2(0f, 128f), new Vector2(440f, 26f), font, TextAnchor.MiddleCenter);
            _guideTitleText.color = new Color(1f, 0.82f, 0.35f);

            // Görsel Kutusu
            GameObject imgFrame = CreatePanel(_guidePanel.transform, "IllustrationFrame", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 54f), new Vector2(176f, 96f), new Color(0.12f, 0.08f, 0.05f, 1f));
            GameObject imgObj = CreatePanel(imgFrame.transform, "Illustration", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(168f, 88f), Color.white);
            _guideIllustrationImage = imgObj.GetComponent<Image>();

            // Metin Kutusu (En fazla 3 cümle)
            _guideBodyText = CreateText(_guidePanel.transform, "GuideBody", "", 10, new Vector2(0f, -44f), new Vector2(430f, 86f), font, TextAnchor.MiddleCenter);
            _guideBodyText.lineSpacing = 1.2f;

            // Alt Gezinme Çubuğu (Geri / Sayfa / İleri / Ana Menü)
            _prevPageButton = CreateMenuButton(_guidePanel.transform, "Btn_Prev", "◀ Geri", new Vector2(-150f, -122f), new Vector2(95f, 30f), font, () => ChangeGuidePage(-1));
            _guidePageIndicatorText = CreateText(_guidePanel.transform, "PageIndicator", "Sayfa 1 / 6", 10, new Vector2(-45f, -122f), new Vector2(90f, 26f), font, TextAnchor.MiddleCenter);
            _nextPageButton = CreateMenuButton(_guidePanel.transform, "Btn_Next", "İleri ▶", new Vector2(45f, -122f), new Vector2(95f, 30f), font, () => ChangeGuidePage(1));
            CreateMenuButton(_guidePanel.transform, "Btn_BackFromGuide", "Ana Menü [Esc]", new Vector2(162f, -122f), new Vector2(115f, 30f), font, ShowMainMenu);
        }

        private void GenerateFallbackSpritesIfNeeded()
        {
            _proceduralGuideSprites = new Sprite[6];
            for (int i = 0; i < 6; i++)
            {
                Texture2D tex = new Texture2D(48, 27, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };
                Color bg = new Color(0.45f, 0.27f, 0.15f);
                Color accent = GuidePages[i].ThemeColor;
                for (int y = 0; y < 27; y++)
                {
                    for (int x = 0; x < 48; x++)
                    {
                        bool border = x == 0 || x == 47 || y == 0 || y == 26;
                        bool iconArea = Mathf.Abs(x - 24) < 10 && Mathf.Abs(y - 13) < 7;
                        tex.SetPixel(x, y, border ? new Color(0.14f, 0.08f, 0.05f) : (iconArea ? accent : bg));
                    }
                }
                tex.Apply();
                _proceduralGuideSprites[i] = Sprite.Create(tex, new Rect(0, 0, 48, 27), new Vector2(0.5f, 0.5f), 16f);
            }
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static Text CreateText(Transform parent, string name, string content, int fontSize, Vector2 anchoredPos, Vector2 size, Font font, TextAnchor align)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            Text txt = go.GetComponent<Text>();
            txt.font = font;
            txt.text = content;
            txt.fontSize = fontSize;
            txt.alignment = align;
            txt.color = new Color(0.96f, 0.91f, 0.84f);
            txt.supportRichText = true;
            return txt;
        }

        private static Button CreateMenuButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size, Font font, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = CreatePanel(parent, name, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPos, size, new Color(0.58f, 0.32f, 0.16f, 1f));
            Button btn = btnObj.AddComponent<Button>();

            ColorBlock cb = btn.colors;
            cb.normalColor      = new Color(0.58f, 0.32f, 0.16f, 1f);
            cb.highlightedColor = new Color(0.78f, 0.45f, 0.22f, 1f);
            cb.pressedColor     = new Color(0.42f, 0.22f, 0.10f, 1f);
            cb.disabledColor    = new Color(0.30f, 0.22f, 0.18f, 0.5f);
            btn.colors = cb;

            btn.onClick.AddListener(onClick);
            CreateText(btnObj.transform, "Label", label, 12, Vector2.zero, size, font, TextAnchor.MiddleCenter);
            return btn;
        }
    }
}