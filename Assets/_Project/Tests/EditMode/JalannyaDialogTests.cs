using System;
using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class JalannyaDialogTests
    {
        static BarisDialog Baris(string teks, string efek = null, bool akhir = false, params PilihanDialog[] pilihan) =>
            new BarisDialog { Pembicara = "Laras", Teks = teks, Efek = efek, Akhir = akhir, Pilihan = pilihan };

        static PilihanDialog Pilihan(string teks, string efek, int lompatKe) =>
            new PilihanDialog { Teks = teks, Efek = efek, LompatKe = lompatKe };

        [Test]
        public void BarisBerjalanBerurutanLaluSelesai()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("satu"), Baris("dua") });
            Assert.AreEqual("satu", d.Sekarang.Teks);
            d.Lanjut();
            Assert.AreEqual("dua", d.Sekarang.Teks);
            d.Lanjut();
            Assert.IsTrue(d.Selesai);
            Assert.IsNull(d.Sekarang);
        }

        [Test]
        public void EfekBarisDijalankanSaatBarisTampil()
        {
            var s = new GameState();
            var d = new JalannyaDialog(s, new[] { Baris("satu"), Baris("dua", "bukti:B4") });
            Assert.IsFalse(s.BuktiKunci[Kode.B4]);
            d.Lanjut();
            Assert.IsTrue(s.BuktiKunci[Kode.B4]);
        }

        [Test]
        public void BarisAkhirMenutupPercakapan()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("satu", akhir: true), Baris("tidak tampil") });
            d.Lanjut();
            Assert.IsTrue(d.Selesai);
        }

        [Test]
        public void PilihanMenjalankanEfekDanMelompat()
        {
            var s = new GameState { MalamKe = 3 };
            var d = new JalannyaDialog(s, new[]
            {
                Baris("Bu Sumi menatap dingin.", null, false,
                    Pilihan("Bu, saya minta maaf soal yang dulu.", "kepercayaan:MintaMaaf", 2),
                    Pilihan("Ibu yang bikin ibu saya sakit, kan?", "tuduh-sumi", -1)),
                Baris("tidak tampil"),
                Baris("Aku ora tau nyantet sapa-sapa."),
            });
            Assert.IsTrue(d.MenungguPilihan);
            d.Pilih(0);
            Assert.AreEqual(2, d.Indeks);
            Assert.AreEqual(1, s.KepercayaanSumi);
        }

        [Test]
        public void PilihanDenganLompatMinusSatuMenutupPercakapan()
        {
            var s = new GameState { MalamKe = 3 };
            var d = new JalannyaDialog(s, new[] { Baris("?", null, false, Pilihan("Tuduh", "tuduh-sumi", -1)) });
            d.Pilih(0);
            Assert.IsTrue(d.Selesai);
            Assert.IsFalse(KepercayaanSumi.PintuTerbuka(s));
        }

        [Test]
        public void LanjutSaatMenungguPilihanDitolak()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("?", null, false, Pilihan("ya", null, -1)) });
            Assert.Throws<InvalidOperationException>(() => d.Lanjut());
        }

        [Test]
        public void DialogKosongDitolak()
        {
            Assert.Throws<ArgumentException>(() => new JalannyaDialog(new GameState(), new BarisDialog[0]));
        }

        [Test]
        public void LompatanDiLuarDaftarDitolak()
        {
            var d = new JalannyaDialog(new GameState(), new[] { Baris("?", null, false, Pilihan("ya", null, 5)) });
            Assert.Throws<ArgumentOutOfRangeException>(() => d.Pilih(0));
        }
    }
}
