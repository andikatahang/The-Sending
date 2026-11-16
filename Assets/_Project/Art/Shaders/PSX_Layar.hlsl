// Custom Function untuk Shader Graph PSX_Layar (URP Fullscreen): kuantisasi warna per kanal + dither Bayer 4x4.
// Node Custom Function: Type = File, Name = PsxLayar, Inputs: Warna (Vector3), PosisiPiksel (Vector2),
// BitWarna (Float), KuatDither (Float); Output: Hasil (Vector3). Precision graph = Single.
// Tidak bergantung pada pustaka lain, jadi sama di Metal (Mac) dan DirectX (Windows).
#ifndef THESENDING_PSX_LAYAR_INCLUDED
#define THESENDING_PSX_LAYAR_INCLUDED

float3 PsxKeSrgb(float3 c)
{
    float3 rendah = c * 12.92;
    float3 tinggi = 1.055 * pow(max(c, 0.0031308), 1.0 / 2.4) - 0.055;
    return lerp(rendah, tinggi, step(0.0031308, c));
}

float3 PsxKeLinear(float3 c)
{
    float3 rendah = c / 12.92;
    float3 tinggi = pow((max(c, 0.04045) + 0.055) / 1.055, 2.4);
    return lerp(rendah, tinggi, step(0.04045, c));
}

void PsxLayar_float(float3 Warna, float2 PosisiPiksel, float BitWarna, float KuatDither, out float3 Hasil)
{
    static const float Bayer4[16] =
    {
         0.0,  8.0,  2.0, 10.0,
        12.0,  4.0, 14.0,  6.0,
         3.0, 11.0,  1.0,  9.0,
        15.0,  7.0, 13.0,  5.0
    };

    float3 warna = saturate(Warna);
    // Kuantisasi di ruang sRGB supaya warna gelap tidak habis jadi hitam (proyek memakai ruang warna Linear).
    #ifndef UNITY_COLORSPACE_GAMMA
    warna = PsxKeSrgb(warna);
    #endif

    // PosisiPiksel = Screen Position mode Pixel: piksel target render (resolusi rendah), jadi pola ikut piksel besar.
    // Jika node memberi posisi ternormalisasi (0..1) alih-alih piksel, ubah ke piksel target render.
    float2 posisi = all(PosisiPiksel <= 1.0) ? PosisiPiksel * _ScreenParams.xy : PosisiPiksel;
    uint2 piksel = uint2(floor(posisi)) & 3;
    float ambang = (Bayer4[piksel.x + piksel.y * 4] + 0.5) / 16.0 - 0.5;
    float tingkat = exp2(BitWarna) - 1.0;
    warna = saturate(floor(warna * tingkat + 0.5 + ambang * KuatDither) / tingkat);

    #ifndef UNITY_COLORSPACE_GAMMA
    warna = PsxKeLinear(warna);
    #endif
    Hasil = warna;
}

#endif
