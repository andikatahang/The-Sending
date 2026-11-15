using UnityEngine;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Membangun uGUI lewat kode, supaya tidak ada prefab UI yang harus disambung manual.
    public static class UiPabrik
    {
        static Font font;
        public static Font Font => font != null ? font : (font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        public static Canvas BuatCanvas(string nama, int urutan)
        {
            var go = new GameObject(nama, typeof(Canvas), typeof(CanvasScaler));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = urutan;
            var skala = go.GetComponent<CanvasScaler>();
            skala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            skala.referenceResolution = new Vector2(1920, 1080);
            skala.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        public static RectTransform Isi(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Text BuatTeks(Transform induk, string nama, int ukuran, TextAnchor rata, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(nama, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(induk, false);
            Isi((RectTransform)go.transform, anchorMin, anchorMax);
            var t = go.GetComponent<Text>();
            t.font = Font;
            t.fontSize = ukuran;
            t.alignment = rata;
            t.color = new Color(0.9f, 0.88f, 0.82f);
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            t.raycastTarget = false;
            return t;
        }

        public static Image BuatPanel(Transform induk, string nama, Color warna, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(nama, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(induk, false);
            Isi((RectTransform)go.transform, anchorMin, anchorMax);
            var img = go.GetComponent<Image>();
            img.color = warna;
            img.raycastTarget = false;
            return img;
        }
    }
}
