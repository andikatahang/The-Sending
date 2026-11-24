using TheSending.Core;
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
            info.text = $"UJI  Malam {gm.State.MalamKe}  Kondisi Ibu {gm.State.KondisiIbu}  [F5] simpan  [F6] jam +30 menit  [F9] tertangkap";
            if (gm.Dijeda) return;

            if (kb.f5Key.wasPressedThisFrame)
                Pesan(gm.SimpanCheckpoint() ? "Checkpoint disimpan." : "Checkpoint GAGAL disimpan.");
            if (kb.f6Key.wasPressedThisFrame)
            {
                JamGim.Majukan(gm.State, 30f);
                Pesan("Jam gim maju 30 menit.");
            }
            if (kb.f9Key.wasPressedThisFrame)
            {
                gm.Tertangkap();
                // Isi scene (lemari, lampu, paku) harus mengikuti state yang baru dimuat.
                gm.MuatScene(gm.SceneUntukState);
            }
        }

        void Pesan(string teks)
        {
            if (hud != null) hud.Tampilkan(teks);
            else Debug.Log(teks, this);
        }
    }
}
