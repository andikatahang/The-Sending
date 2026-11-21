using System.IO;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Melukis tekstur tokoh 256 px di atas tata letak UV character_256.png (koordinat x, y dari kiri atas).
    // Tiap piksel digolongkan (baju, bawahan, kulit, rambut, sepatu) lalu diwarnai ulang dengan bayangan aslinya.
    public static partial class PembuatPengganti
    {
        enum Kelas { Baju, Bawah, Kulit, Rambut, Sepatu, Lain }
        const int Sisi = 256;

        static Kelas Golongkan(int x, int y, Color c)
        {
            float mx = Mathf.Max(c.r, c.g, c.b);
            bool kulit = c.r > 0.43f && c.r - c.b > 0.18f && c.r >= c.g;
            if (y < 97 && !(x > 172 && y > 68)) return kulit ? Kelas.Kulit : Kelas.Baju;   // kaos
            if (x < 112 && y > 200) return Kelas.Sepatu;
            if (Wajah(x, y)) return x >= 229 && mx < 0.35f ? Kelas.Rambut : Kelas.Kulit;    // wajah dicat ulang (tanpa kacamata asli)
            if (x > 150 && y > 55 && y < 190 && mx < 0.35f) return Kelas.Rambut;
            if (x >= 147 && x < 175 && y >= 100 && y < 157) return Kelas.Lain;                 // daerah lensa, tak dipakai
            if (kulit) return Kelas.Kulit;
            if (y < 157 && x < 147) return Kelas.Bawah;                                        // celana atas
            if (x >= 112 && x < 200 && y > 200) return Kelas.Bawah;                            // celana bawah
            if (c.b > c.r + 0.03f) return Kelas.Bawah;
            return Kelas.Kulit;
        }

        static bool Wajah(int x, int y) => x >= 194 && x <= 232 && y >= 100 && y <= 140;
        static bool OvalWajah(int x, int y) => Mathf.Pow((x - 211) / 19f, 2) + Mathf.Pow((y - 120) / 16f, 2) <= 1f;
        static bool Kepala(int x, int y) => x >= 172 && y > 55 && y < 190;

        static float Terang(Color c) => 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;

        static Color Hex(string hex) => ColorUtility.TryParseHtmlString("#" + hex, out var w) ? w : throw new System.FormatException(hex);

        static bool Bermotif(Pola p, int x, int y) => p switch
        {
            Pola.Batik => (x + y) % 8 == 0 || (x - y + Sisi) % 8 == 0,
            Pola.Bunga => ((x + y / 7 * 3) % 7 is 3 or 4) && (y % 7 is 3 or 4),
            Pola.Kotak => x / 5 % 3 == 0 || y / 5 % 3 == 0,
            _ => false,
        };

        static Texture2D Lukis(Tokoh t)
        {
            var asal = new Texture2D(2, 2);
            asal.LoadImage(File.ReadAllBytes(JalurTeksturDasar));
            var px = asal.GetPixels();   // baris dari bawah
            Color Ambil(int x, int y) => px[(Sisi - 1 - y) * Sisi + x];

            // Rata-rata kecerahan dan warna kulit asli, supaya bayangan dipertahankan relatif.
            var jumlah = new float[6]; var banyak = new int[6]; var kulitAsli = Color.black;
            for (int y = 0; y < Sisi; y++)
                for (int x = 0; x < Sisi; x++)
                {
                    var c = Ambil(x, y); int k = (int)Golongkan(x, y, c);
                    jumlah[k] += Terang(c); banyak[k]++;
                    if (k == (int)Kelas.Kulit) kulitAsli += c;
                }
            kulitAsli /= Mathf.Max(1, banyak[(int)Kelas.Kulit]);
            float Rata(Kelas k) => Mathf.Max(0.01f, jumlah[(int)k] / Mathf.Max(1, banyak[(int)k]));

            var hasil = new Color[px.Length];
            for (int y = 0; y < Sisi; y++)
                for (int x = 0; x < Sisi; x++)
                {
                    var c = Ambil(x, y); var k = Golongkan(x, y, c);
                    float bayang = Terang(c) / Rata(k);
                    Color w;
                    if (t.Siluet) w = Color.Lerp(Color.black, c, 0.06f);
                    else switch (k)
                    {
                        case Kelas.Baju: w = Kain(t.Baju, t.Aksen, t.PolaBaju, x, y, bayang); break;
                        case Kelas.Bawah: w = Kain(t.Bawah, t.Aksen, t.PolaBawah, x, y, bayang); break;
                        case Kelas.Rambut:
                            w = t.WarnaKerudung != null && Kepala(x, y) ? Hex(t.WarnaKerudung) * Mathf.Clamp(bayang, 0.85f, 1.1f)
                                : Hex(t.Rambut) * Mathf.Clamp(bayang, 0.6f, 1.5f);
                            break;
                        case Kelas.Sepatu: w = Terang(c) > 0.5f ? c * 0.7f : Hex(t.Sepatu) * (0.6f + Terang(c)); break;
                        case Kelas.Kulit:
                            var target = Hex(t.Kulit);
                            if (t.Pucat) target = Color.Lerp(target, new Color(0.8f, 0.8f, 0.76f), 0.4f);
                            w = Wajah(x, y)   // diratakan: bayangan jenggot dan bingkai kacamata asli hilang
                                ? target * Mathf.Clamp(bayang, 0.92f, 1.05f)
                                : new Color(c.r * target.r / kulitAsli.r, c.g * target.g / kulitAsli.g, c.b * target.b / kulitAsli.b);
                            if (t.WarnaKerudung != null && Kepala(x, y) && x >= 190 && !OvalWajah(x, y))
                                w = Hex(t.WarnaKerudung) * Mathf.Clamp(bayang, 0.85f, 1.1f);
                            break;
                        default: w = c; break;
                    }
                    w.a = 1f;
                    hasil[(Sisi - 1 - y) * Sisi + x] = w;
                }

            if (!t.Siluet) Wajah(t, hasil);
            var teks = new Texture2D(Sisi, Sisi, TextureFormat.RGBA32, false);
            teks.SetPixels(hasil);
            teks.Apply();
            return teks;
        }

        static Color Kain(string dasar, string aksen, Pola pola, int x, int y, float bayang) =>
            (Bermotif(pola, x, y) ? Hex(aksen) : Hex(dasar)) * Mathf.Clamp(bayang, 0.85f, 1.1f);

        // Wajah di UV terputar 90 derajat: dagu ke kiri (x 195), ubun-ubun ke kanan; mata di x 222-223, y 111-112 dan 126-127.
        static void Wajah(Tokoh t, Color[] h)
        {
            void Cat(int x0, int x1, int y0, int y1, Color w)
            {
                for (int y = y0; y <= y1; y++) for (int x = x0; x <= x1; x++) h[(Sisi - 1 - y) * Sisi + x] = w;
            }
            void Gelapkan(int x0, int x1, int y0, int y1, float f)
            {
                for (int y = y0; y <= y1; y++) for (int x = x0; x <= x1; x++) { int i = (Sisi - 1 - y) * Sisi + x; h[i] *= f; h[i].a = 1f; }
            }
            var gelap = new Color(0.08f, 0.06f, 0.05f);
            var alis = t.Tua ? Hex(t.Rambut) * 0.8f : gelap;
            foreach (int ya in new[] { 110, 125 })
            {
                Cat(222, 223, ya, ya + 3, new Color(0.85f, 0.82f, 0.78f));   // putih mata
                Cat(222, 223, ya + 1, ya + 2, gelap);                         // pupil
                if (t.Wanita) { Cat(225, 225, ya, ya + 3, gelap); Cat(227, 227, ya, ya + 4, alis); }   // bulu mata, alis tipis
                else Cat(226, 227, ya - 1, ya + 4, alis);                                              // alis tebal
            }
            Gelapkan(212, 213, 118, 121, 0.85f);   // hidung
            if (t.Wanita && !t.Tua) Cat(203, 204, 117, 122, new Color(0.6f, 0.25f, 0.27f));
            else Gelapkan(204, 204, 117, 122, 0.7f);
            if (t.Tua)
                foreach (var (x, y0, y1) in new[] { (207, 112, 114), (207, 125, 127), (219, 110, 114), (219, 125, 129) })
                    Gelapkan(x, x, y0, y1, 0.8f);
            if (t.Kacamata)
            {
                foreach (var (ya, yb) in new[] { (107, 116), (123, 132) })
                    for (int y = ya; y <= yb; y++) for (int x = 219; x <= 226; x++)
                        if (y == ya || y == yb || x == 219 || x == 226) Cat(x, x, y, y, gelap);
                Cat(223, 223, 117, 122, gelap);
            }
        }
    }
}
