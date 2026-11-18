using TheSending.Core;
using UnityEngine;

namespace TheSending.Runtime
{
    // K4 opsi B: duduk di kursi samping ranjang Ibu melompatkan jam ke kejadian Malam 1 berikutnya (JamGim.Tunggu).
    public class KursiTunggu : Interaktif
    {
        const float LamaKartu = 2f;

        [SerializeField] KartuTeks kartu;

        public override bool BisaDiinteraksi =>
            GameManager.Instance != null && !kartu.Tampil && JamGim.KejadianBerikutnya(GameManager.Instance.State) != null;

        public override string Prompt
        {
            get
            {
                var sampai = GameManager.Instance != null ? JamGim.KejadianBerikutnya(GameManager.Instance.State) : null;
                return sampai == null ? base.Prompt : $"Duduk menunggu sampai {JamGim.Format(sampai.Value)}";
            }
        }

        public override void Interaksi()
        {
            var s = GameManager.Instance.State;
            if (!JamGim.Tunggu(s)) return;
            // Kartu menjeda gim sampai pudar, jadi pengingat obat atau cutscene 23.00 baru jalan setelahnya.
            kartu.StartCoroutine(kartu.Tampilkan(JamGim.Format(s.MenitGim), "Raka duduk menunggu di samping Ibu.", LamaKartu));
        }
    }
}
