# Revisi Teks Fase 4 (sebelum → sesudah)

Dari umpan balik playtest: instruksi kurang jelas, bahasa kaku. Kutipan wajib Game Story, kalimat Jawa, kode efek, dan isi Jurnal tidak diubah. AlurProlog.cs tidak disentuh (ada perubahan lain yang belum di-commit).

## 1. Instruksi, notifikasi HUD, tujuan, prompt

| File | Sebelum | Sesudah |
|---|---|---|
| `AlurMalam2.cs` | Tekan C untuk membuka kompas HP. Jarumnya berputar di dekat benda santet. | Tekan [C] untuk membuka kompas. Jarumnya berputar dekat benda santet. |
| `AlurMalam2.cs` | Papan lantai di dekat ranjang berbunyi kopong. | Papan lantai dekat ranjang Ibu bunyinya kopong. Butuh linggis. |
| `AlurMalam2.cs` | Pintu gudang terkunci. Laras mungkin menyimpan kuncinya. | Gudang terkunci. Minta kuncinya ke Laras di ruang tengah. |
| `AlurMalam2.cs` | Gudang gelap. Nyalakan senter (F). | Gudang gelap. Tekan [F] untuk menyalakan senter. |
| `AlurMalam2.cs` | Linggis bersandar di dinding gudang. | Linggis bersandar di dinding gudang. Ambil dengan [E]. |
| `AlurMalam2.cs` | Boneka itu otomatis difoto dan masuk Papan Bukti. | Boneka itu sudah kufoto. Lihat Papan Bukti di Jurnal [Tab]. |
| `AlurMalam2.cs` | Kursi di kamar Ibu bisa dipakai menunggu. | Duduk di kursi kamar Ibu [E] untuk menunggu. |
| `AlurMalam2.cs` | Kembali ke kamar Raka tanpa ketahuan. | Aku harus kembali ke kamarku tanpa ketahuan. |
| `AlurMalam2.cs` | Sosok itu mendengar langkah. Jangan berlari. | Sosok itu mendengar langkah. Jangan lari [Shift], jalan pelan. |
| `AlurMalam2.cs` | Bersembunyi di lemari (E). Tahan Spasi untuk menahan napas. | Sembunyi di lemari dengan [E]. Tahan [Spasi] untuk menahan napas. |
| `AlurRumahBuSumi.cs` | Bu Sumi membukakan pintu dan berdiri menghalangi. Dapurnya gelap; teploknya mati. | Bu Sumi berdiri menghadang. Dapurnya gelap, teploknya mati. Ajak bicara [E]. |
| `AlurRumahBuSumi.cs` | Media santet dinetralkan di anglo dapur rumah. | Bawa pulang. Media santet dinetralkan di anglo dapur rumah. |
| `AlurRumahBuSumi.cs` | Raka belum bicara dengan Bu Sumi. | Aku harus bicara dulu dengan Bu Sumi [E]. | (x2)
| `AlurRumahBuSumi.cs` | Beri tahu Bu Sumi. | Sudah beres. Kembali dan beri tahu Bu Sumi [E]. |
| `AlurRumahBuSumi.cs` | Bu Sumi masih butuh bantuan (teplok, air). Raka tidak kembali lagi malam ini. Tekan E sekali lagi untuk keluar. | Teplok dan air Bu Sumi belum beres. Kalau keluar, aku tak bisa kembali malam ini. Tekan [E] lagi untuk tetap keluar. |
| `JadwalObat.cs` | Waktunya obat Ibu. | Waktunya memberi obat Ibu di kamarnya. |
| `KetukSaka.cs` | Saka guru ini berbunyi padat. | Bunyinya padat. Bukan saka yang ini. |
| `AlurMalam3.cs` | Paku-paku itu otomatis difoto dan masuk Papan Bukti. | Paku-paku itu sudah kufoto. Lihat Papan Bukti di Jurnal [Tab]. |
| `AlurMalam3.cs` | Paku santet tidak boleh dicabut sembarangan. Mungkin Bapak menulis sesuatu; periksa gudang. | Paku santet tak boleh dicabut asal. Bapak mungkin mencatat caranya; cari di gudang. |
| `AlurMalam3.cs` | Pakde pergi dengan wajah masam. Raka masuk ke kamarnya. | Pakde pergi dengan muka masam. Aku masuk ke kamarku. |
| `AlurMalam3.cs` | Ada kotak sepatu lama di dekat dinding kamar. | Ada kotak sepatu lama di kamarku. Buka dengan [E]. |
| `AlurMalam3.cs` | Raka belum tahu cara menetralkan paku itu. Mungkin Bapak menulisnya; periksa gudang. | Aku belum tahu cara menetralkan paku ini. Cari catatan Bapak di gudang. |
| `AlurMalam3.cs` | Bahan ritual belum lengkap. Halaman 4 menyebut tempatnya (Jurnal, Tab). | Bahan ritual belum lengkap. Tempatnya ada di Halaman 4 (Jurnal [Tab]). |
| `AlurMalam3.cs` | Bahan ritual sudah lengkap. Anglo ada di dapur. | Bahan ritual sudah lengkap. Mulai ritual di anglo dapur [E]. |
| `AlurMalam3.cs` | Raka teringat Bu Sumi, peracik jamu di dekat jembatan. Keluar lewat pintu depan. | Aku teringat Bu Sumi, peracik jamu dekat jembatan. Keluar lewat pintu depan. |
| `AlurMalam3.cs` | Kursi di kamar Ibu bisa dipakai menunggu. | Duduk di kursi kamar Ibu [E] untuk menunggu. |
| `AlurMalam3.cs` | Buka kompas (C). Jarumnya berputar ke arah saka guru di ruang tengah. | Tekan [C] untuk membuka kompas. Jarumnya berputar dekat saka guru di ruang tengah. |
| `AlurMalam3.cs` | Paku itu tidak mau lepas. Ibu mengerang dari kamarnya. | Paku itu tidak mau lepas; urutannya salah. Ibu mengerang dari kamarnya. |
| `AlurMalam3.cs` | Gemerincing paku terdengar dari dapur. Bersembunyi (E di lemari) dan tahan napas (Spasi). | Gemerincing paku dari dapur! Sembunyi di lemari [E], tahan napas [Spasi]. |
| `AlurMalam3.cs` | Raka tidak bisa pergi selama Ibu terus menjerit. | Aku tak bisa pergi selama Ibu menjerit. Cabut dulu paku di saka. |
| `AlurMalam3.cs` | Sudah larut. Raka tidak keluar lagi malam ini. | Sudah larut. Aku tidak keluar lagi malam ini. |
| `KerekanSumur.cs` | Butuh tali dan pengait dari pos ronda. | Aku butuh tali dan pengait. Pinjam di pos ronda desa. |
| `KerekanSumur.cs` | Terlalu cepat. Bungkusan itu jatuh lagi ke dasar sumur. | Terlalu cepat! Bungkusannya jatuh lagi. Tarik lebih pelan. |
| `PapanLantai.cs` | Papan ini berbunyi kopong. Butuh alat untuk membongkarnya. | Papan ini kopong. Butuh linggis untuk membongkarnya. |
| `AlurMalam4.cs` | Ada sesuatu di dasar sumur belakang. Butuh tali dan pengait; para peronda di pos ronda punya. | Ada sesuatu di dasar sumur belakang. Pinjam tali dan pengait di pos ronda. |
| `AlurMalam4.cs` | Tali dan pengait sudah ada. Sumur di pekarangan belakang, lewat pintu dapur. | Tali dan pengait sudah ada. Ke sumur belakang lewat pintu dapur. |
| `AlurMalam4.cs` | Kursi di kamar Ibu bisa dipakai menunggu. | Duduk di kursi kamar Ibu [E] untuk menunggu. |
| `AlurMalam4.cs` | Bungkusan itu bisa dinetralkan di anglo dapur. | Netralkan bungkusan itu di anglo dapur [E]. |
| `NaikPlafon.cs` | Butuh tangga dari gudang. | Terlalu tinggi. Ambil tangga bambu di gudang. |
| `AlurDesaMalam3.cs` | Ritual Penangkal masih bisa dipelajari dari Halaman 4 buku harian Bapak. | Cara ritualnya masih ada di Halaman 4 buku harian Bapak (Jurnal [Tab]). |
| `AlurDesaMalam3.cs` | Rumah keluarga ada di ujung utara jalan. | Saatnya pulang. Rumah ada di ujung utara jalan. |
| `AlurDesaMalam3.cs` | Raka belum menemui Bu Sumi. | Aku harus menemui Bu Sumi dulu. Rumahnya di selatan, kanan jalan. |
| `AlurPrologRumah.cs` | Tekan I untuk melihat inventaris. | Tekan [I] untuk melihat inventaris. |
| `AlurPrologRumah.cs` | Tekan Tab untuk membuka Jurnal. | Tekan [Tab] untuk membuka Jurnal: tujuan, bukti, dan catatan. |
| `AlurMalam1.cs` | Obat Ibu diberikan pukul 21.00. Jam ada di kanan atas. | Obat Ibu harus diberikan pukul 21.00. Lihat jam di kanan atas. |
| `AlurMalam1.cs` | Arahkan ke paku di lantai, lalu tekan Q untuk memotretnya. | Arahkan kamera ke paku di lantai, lalu tekan [Q] untuk memotret. |
| `AlurMalam1.cs` | Foto petunjuk masuk ke Papan Bukti di Jurnal (Tab). | Foto petunjuk masuk Papan Bukti di Jurnal [Tab]. |
| `AlurMalam1.cs` | Masih ada yang belum diperiksa: lemari Bapak di kamar Ibu. | Lemari Bapak di kamar Ibu belum kubuka. Kodenya: tahun rumah ini berdiri. |
| `PakuDiSaka.cs` | Pakunya tertanam dalam. Butuh tang. | Pakunya tertanam dalam. Butuh tang dari gudang. |
| `Hud.cs` | Bukti baru masuk Papan Bukti. | Bukti baru masuk Papan Bukti (Jurnal [Tab]). |
| `Hud.cs` | Papan Bukti diperbarui. | Papan Bukti diperbarui (Jurnal [Tab]). |
| `Hud.cs` | Halaman buku harian ditemukan. | Halaman buku harian ditemukan. Baca di Jurnal [Tab]. |
| `Hud.cs` | Barang baru masuk inventaris (I). | Barang baru masuk inventaris [I]. |
| `Hud.cs` | Foto masuk Papan Bukti. | Foto masuk Papan Bukti (Jurnal [Tab]). |
| `RitualDapur.cs` | Ritual gagal: urutannya salah. Media diangkat dari anglo; ulangi dari awal. | Ritual gagal, urutannya salah. Ulangi dari awal: letakkan media di anglo. |
| `RitualDapur.cs` | Raka belum tahu cara menetralkan media santet. | Aku belum tahu cara menetralkan media santet. |
| `RitualDapur.cs` | Bahan ritual belum lengkap: garam, daun kelor, dan bilah bambu kuning. | Bahannya belum lengkap: garam, daun kelor, bilah bambu kuning. |
| `RitualDapur.cs` | Media menjadi abu. Kubur abunya di pekarangan belakang. | Media menjadi abu. Kubur abunya di pekarangan belakang, lewat pintu dapur. |
| `TujuanMalam.cs` | Masuk ke rumah keluarga | Pulang: masuk ke rumah di ujung jalan |
| `TujuanMalam.cs` | Bicara dengan Bidan Wulan | Bicara dengan Bidan Wulan di depan kamar Ibu |
| `TujuanMalam.cs` | Tunggu sampai pukul 23.00 | Tunggu pukul 23.00 (duduk di kursi kamar Ibu) |
| `TujuanMalam.cs` | Foto paku di lantai kamar Ibu (Q) | Potret paku di lantai kamar Ibu [Q] |
| `TujuanMalam.cs` | Bongkar papan lantai dan baca tulisan boneka | Bongkar papan dekat ranjang Ibu, baca tulisan boneka |
| `TujuanMalam.cs` | Sembunyi dari Sosok, lalu capai kamar Raka | Kembali ke kamar Raka tanpa ketahuan Sosok |
| `TujuanMalam.cs` | Temui Bu Sumi | Temui Bu Sumi di desa (lewat pintu depan) |
| `TujuanMalam.cs` | Kubur abu ritual di pekarangan belakang | Netralkan media di anglo dapur, kubur abunya |
| `TujuanMalam.cs` | Ambil bungkusan di dasar sumur | Ambil bungkusan di dasar sumur belakang |
| `TujuanMalam.cs` | Lolos dari kejaran Sosok | Lari dari Sosok ke pintu belakang dapur |
| `TujuanMalam.cs` | Ketuk saka guru di ruang tengah (E) sampai terdengar berongga | Ketuk saka guru di ruang tengah [E], cari yang berongga |
| `TujuanMalam.cs` | $"Beri obat Ibu pukul {JamGim.Format(JamGim.MulaiJendelaObat(s.MalamKe))} | $"Beri obat Ibu pukul {JamGim.Format(JamGim.MulaiJendelaObat(s.MalamKe))} di kamarnya |
| `PembuatMalam2.cs` | prompt", "Papan lantai | prompt", "Periksa papan lantai |
| `PembuatMalam4.cs` | prompt", "Kerekan sumur | prompt", "Periksa sumur |

## 2. Dialog

| File | Sebelum | Sesudah |
|---|---|---|
| `PembuatDataFase1.cs` | Bu Bidan Wulan lagi periksa Ibu di kamar. Temui beliau dulu, Mas. | Bu Bidan Wulan lagi meriksa Ibu di kamar. Mas temui dulu, gih. |
| `PembuatDataFase1.cs` | Di balik bingkai foto ini ada tulisan tangan. | Eh, di balik bingkai foto ini ada tulisan tangan. |
| `PembuatDataFase1.cs` | Kodenya adalah tahun rumah ini berdiri. | Kodenya: tahun rumah ini berdiri. |
| `PembuatDataFase1.cs` | Bukan cuma itu. Sejak Ibu sakit, orang-orang desa mulai bisik-bisik soal Bu Sumi. | Bukan cuma itu, Mas. Sejak Ibu sakit, orang desa pada bisik-bisik soal Bu Sumi. |
| `PembuatDataFase2.cs` | Ada papan lantai yang bunyinya kopong di dekat ranjang. Kita punya linggis? | Papan lantai dekat ranjang bunyinya kopong. Kita punya linggis, Ras? |
| `PembuatDataFase2.cs` | Tidak usah, Mbah. Biar saya simpan dulu. | Nggak usah, Mbah. Biar saya simpan dulu. |
| `PembuatDataFase3.cs` | Tanah itu peninggalan Bapak, Pakde. Dulu Bapak menolak menjualnya. | Itu tanah peninggalan Bapak, Pakde. Bapak dulu nggak mau menjualnya. |
| `PembuatDataFase3.cs` | Maaf, Pakde. Tanahnya tidak dijual. | Maaf, Pakde. Tanahnya nggak akan dijual. |
| `PembuatDataFase3.cs` | Ada satu halaman buku harian terselip di dasarnya. | Di dasarnya nyelip selembar halaman buku harian. |
| `PembuatDataFase3.cs` | Darmo marah besar dan bilang Bapak akan menyesal... Sama dengan omongan Pakde tadi. | Pakde Darmo marah besar, bilang Bapak bakal menyesal... Persis omongan Pakde tadi. |
| `PembuatDataFase3.cs` | Ada halaman buku harian Bapak terselip di rak alat. | Ada halaman buku harian Bapak nyelip di rak alat. |
| `PembuatDataFase3.cs` | Bapak menulis cara mencabut paku santet dan cara menetralkan medianya. Isinya bisa dibaca di Jurnal. | Bapak nulis cara mencabut paku santet dan menetralkan medianya. Bisa kubaca lagi di Jurnal. |
| `PembuatDataFase3.cs` | Waktu itu uangnya dipakai untuk biaya kuliah saya, Bu. | Waktu itu uangnya kepakai buat biaya kuliah saya, Bu. |
| `PembuatDataFase4.cs` | Pak Joko, saya mau pinjam tali. Tali timba sumur kami putus. | Pak Joko, boleh saya pinjam tali? Tali timba sumur di rumah putus. |
| `PembuatDataFase4.cs` | Belum tentu, Pak. Rambut uban bukan cuma milik Bu Sumi. | Belum tentu, Pak. Yang ubanan kan bukan cuma Bu Sumi. |
| `PembuatDataFase4.cs` | Ayam hitam legam begini tidak dipelihara sembarang orang. | Ayam hitam legam begini nggak sembarang orang punya. |
