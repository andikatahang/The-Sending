using NUnit.Framework;
using TheSending.Core;

namespace TheSending.Tests
{
    public class PemeriksaDialogTests
    {
        static PilihanDialog P(string efek, int lompat) => new PilihanDialog { Teks = "x", Efek = efek, LompatKe = lompat };

        [Test]
        public void DialogSehatTanpaMasalah()
        {
            var baris = new[]
            {
                new BarisDialog { Teks = "a", Efek = "katanya:K1", Pilihan = new[] { P("bukti:B2", 1), P("", -1) } },
                new BarisDialog { Teks = "b", Akhir = true },
            };
            CollectionAssert.IsEmpty(PemeriksaDialog.Periksa(baris));
        }

        [Test]
        public void DialogKosongDilaporkan()
        {
            Assert.AreEqual(1, PemeriksaDialog.Periksa(new BarisDialog[0]).Count);
            Assert.AreEqual(1, PemeriksaDialog.Periksa(null).Count);
        }

        [Test]
        public void EfekSalahDilaporkanDenganTempatnya()
        {
            var baris = new[]
            {
                new BarisDialog { Teks = "a" },
                new BarisDialog { Teks = "b", Efek = "bukti:B9", Pilihan = new[] { P("salah", -1) } },
            };
            var masalah = PemeriksaDialog.Periksa(baris);
            Assert.AreEqual(2, masalah.Count);
            StringAssert.StartsWith("Baris 1:", masalah[0]);
            StringAssert.StartsWith("Baris 1 pilihan 1:", masalah[1]);
        }

        [Test]
        public void LebihDariEmpatPilihanDilaporkan()
        {
            var baris = new[]
            {
                new BarisDialog { Teks = "a", Pilihan = new[] { P("", -1), P("", -1), P("", -1), P("", -1), P("", -1) } },
            };
            Assert.AreEqual(1, PemeriksaDialog.Periksa(baris).Count);
        }

        [Test]
        public void LompatanDiLuarDaftarDilaporkan()
        {
            var baris = new[] { new BarisDialog { Teks = "a", Pilihan = new[] { P("", 1) } } };
            Assert.AreEqual(1, PemeriksaDialog.Periksa(baris).Count);
        }
    }
}
