using TheSending.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace TheSending.Runtime
{
    // Menyimpan Pengaturan ke PlayerPrefs dan menerapkannya ke Audio Mixer dan layar. Tanpa state.
    public static class PenyimpanPengaturan
    {
        const string Kunci = "pengaturan";
        const int LebarJendela = 1280, TinggiJendela = 720;

        // Assets/_Project/Audio/Resources/MixerUtama.mixer, dibuat manual oleh tim (lihat docs/style-guide.md).
        public const string JalurMixer = "MixerUtama";

        public static bool AdaMixer => Resources.Load<AudioMixer>(JalurMixer) != null;

        public static Pengaturan Muat() => Pengaturan.DariJson(PlayerPrefs.GetString(Kunci, ""));

        public static void Simpan(Pengaturan p)
        {
            PlayerPrefs.SetString(Kunci, p.KeJson());
            PlayerPrefs.Save();
        }

        public static void Terapkan(Pengaturan p)
        {
            var mixer = Resources.Load<AudioMixer>(JalurMixer);
            if (mixer == null)
            {
                // shortcut: tanpa mixer hanya volume Master yang berlaku, buat MixerUtama sebelum audio pertama masuk (Fase 1).
                AudioListener.volume = p.Volume[0];
            }
            else
            {
                AudioListener.volume = 1f;
                for (int i = 0; i < Pengaturan.NamaGrup.Length; i++)
                {
                    string nama = "Vol" + Pengaturan.NamaGrup[i];
                    if (!mixer.SetFloat(nama, Pengaturan.KeDesibel(p.Volume[i])))
                        Debug.LogWarning($"Parameter '{nama}' belum diekspos di MixerUtama.", mixer);
                }
            }

            if (Application.isEditor || Screen.fullScreen == p.LayarPenuh) return;
            if (p.LayarPenuh)
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
            else
                Screen.SetResolution(LebarJendela, TinggiJendela, FullScreenMode.Windowed);
        }
    }
}
