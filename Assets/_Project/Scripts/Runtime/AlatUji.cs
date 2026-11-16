using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TheSending.Runtime
{
    // Tombol uji untuk daftar cek save (CLAUDE.md). Hanya hidup di editor dan development build.
    public class AlatUji : MonoBehaviour
    {
        [SerializeField] Hud hud;

        Text info;

        void Awake()
        {
            if (!Debug.isDebugBuild)
            {
                enabled = false;
                return;
            }
            var akar = UiPabrik.BuatCanvas("AlatUji", 12).transform;
            akar.SetParent(transform, false);
            info = UiPabrik.BuatTeks(akar, "Info", 22, TextAnchor.UpperLeft, new Vector2(0.02f, 0.92f), new Vector2(0.7f, 0.98f));
        }

        void Update()
        {
            var gm = GameManager.Instance;
            var kb = Keyboard.current;
            if (gm == null || kb == null) return;
            info.text = $"UJI  Malam {gm.State.MalamKe}  Kondisi Ibu {gm.State.KondisiIbu}  [F5] simpan checkpoint  [F9] tertangkap";
            if (gm.Dijeda) return;

            if (kb.f5Key.wasPressedThisFrame)
                Pesan(gm.SimpanCheckpoint() ? "Checkpoint disimpan." : "Checkpoint GAGAL disimpan.");
            if (kb.f9Key.wasPressedThisFrame)
            {
                gm.Tertangkap();
                Pesan("Tertangkap: checkpoint dimuat, Kondisi Ibu -5.");
            }
        }

        void Pesan(string teks)
        {
            if (hud != null) hud.Tampilkan(teks);
            else Debug.Log(teks, this);
        }
    }
}
