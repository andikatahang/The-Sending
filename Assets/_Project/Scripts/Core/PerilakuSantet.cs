namespace TheSending.Core
{
    public enum KeadaanSantet { Patroli, Selidiki, Kejar }

    // Yang ditangkap indra Sosok Santet dalam satu frame. Jarak = jarak datar (meter) ke pemain.
    public struct Pengamatan
    {
        public float Jarak;
        public Gerak Gerak;
        public bool TahanNapas, MinyakSereh, Bersembunyi, SenterMenyala, SenterMengarah;
    }

    // Otak Sosok Santet (desain 2.4): Patroli, Selidiki (menuju suara atau cahaya terakhir), Kejar.
    // Penglihatannya buruk (Game Story 4.2): ia hanya mendengar langkah dan napas, dan melihat cahaya senter yang mengenainya.
    // Semua jarak dan lama adalah nilai awal; disetel saat playtest.
    public class PerilakuSantet
    {
        public const float JarakTangkap = 1f, JarakKejar = 3f, JarakLepas = 8f, LamaSelidiki = 6f;

        float sisaSelidiki;

        public KeadaanSantet Keadaan { get; private set; } = KeadaanSantet.Patroli;

        // Malam 7: langsung mengejar tanpa menunggu deteksi.
        public void MulaiKejar() => Keadaan = KeadaanSantet.Kejar;

        // Pemain yang bersembunyi hanya bisa ketahuan dari napasnya (Game Story 6.6).
        public static bool Mendeteksi(Pengamatan p) =>
            p.Bersembunyi
                ? KewaspadaanSantet.Terdengar(p.Jarak, Gerak.Diam, p.TahanNapas, p.MinyakSereh)
                : KewaspadaanSantet.Terdengar(p.Jarak, p.Gerak, p.TahanNapas, p.MinyakSereh)
                  || KewaspadaanSantet.CahayaTerlihat(p.Jarak, p.SenterMenyala, p.SenterMengarah);

        // true = pemain tertangkap pada frame ini.
        public bool Tick(float detik, Pengamatan p)
        {
            // Yang sedang dikejar dan terjangkau tertangkap walau diam menahan napas; hanya bersembunyi yang menyelamatkan.
            if (Keadaan == KeadaanSantet.Kejar && !p.Bersembunyi && p.Jarak <= JarakTangkap) return true;
            if (Mendeteksi(p))
            {
                if (p.Jarak <= JarakTangkap) return true;
                bool kejar = !p.Bersembunyi && (p.Jarak <= JarakKejar || Keadaan == KeadaanSantet.Kejar);
                Keadaan = kejar ? KeadaanSantet.Kejar : KeadaanSantet.Selidiki;
                sisaSelidiki = LamaSelidiki;
                return false;
            }
            if (Keadaan == KeadaanSantet.Kejar && (p.Bersembunyi || p.Jarak > JarakLepas))
            {
                Keadaan = KeadaanSantet.Selidiki;
                sisaSelidiki = LamaSelidiki;
            }
            else if (Keadaan == KeadaanSantet.Selidiki)
            {
                sisaSelidiki -= detik;
                if (sisaSelidiki <= 0f) Keadaan = KeadaanSantet.Patroli;
            }
            return false;
        }
    }
}
