# Vosk ile C# Ses Tanýma Projesi

Bu proje, C# Windows Forms, [Vosk API](https://alphacephei.com/vosk/) ve [NAudio](https://github.com/naudio/NAudio) kütüphanelerini kullanarak gerçek zamanlý ses tanýma (Speech-to-Text) iþlemi yapar.

## Gereksinimler

* Visual Studio (C# .NET Framework projesi)
* [Vosk NuGet Paketi](https://www.nuget.org/packages/Vosk/)
* [NAudio NuGet Paketi](https://www.nuget.org/packages/NAudio/)
* Harici Vosk Dil Modeli

## Kurulum

1.  Projeyi klonlayýn.
2.  Visual Studio'da açýn ve NuGet paketlerinin geri yüklenmesini bekleyin (Restore NuGet Packages).
3.  **Vosk Dil Modelini Ýndirin:**
    * Vosk, çalýþmak için bir dil modeline ihtiyaç duyar. Türkçe model (veya baþka bir dil) için [Vosk Modelleri Sayfasýný](https://alphacephei.com/vosk/models) ziyaret edin.
    * Küçük Türkçe modeli (örn: `vosk-model-small-tr-0.3`) indirin ve .zip dosyasýndan çýkarýn.
4.  **Model Yolunu Güncelleyin:**
    * `MainForm.cs` dosyasýný açýn.
    * `MODEL_PATH` sabitini, 3. adýmda indirdiðiniz model klasörünün konumuyla deðiþtirin.
        ```csharp
        private const string MODEL_PATH = @"C:\vosk-model-small-tr-0.3"; // <-- BU YOLU KENDÝNÝZE GÖRE DEÐÝÞTÝRÝN
        ```

## ÖNEMLÝ: `0x8007000B` Baþlatma Hatasý Çözümü

Eðer uygulamayý baþlattýðýnýzda **"Vosk baþlatma hatasý: Geçersiz biçimdeki bir program yüklenmek istendi. (HRESULT: 0x8007000B)"** þeklinde bir hata alýyorsanýz, bunun nedeni projenizin 32-bit (x86) olarak çalýþmaya çalýþmasý, ancak Vosk kütüphanesinin 64-bit (x64) olmasýdýr.

Bu mimari uyuþmazlýðýný çözmek için projeyi **x64**'e zorlamanýz gerekir:

1.  Visual Studio'da, Solution Explorer panelinde proje adýna (`SpeechRecognition`) sað týklayýn ve **Properties** (Özellikler) seçeneðine gidin.
2.  Sol menüden **Build** (Oluþtur) sekmesine týklayýn.
3.  Açýlan ekranda **Platform target** (Platform hedefi) ayarýný bulun.
4.  Bu ayarý `Any CPU` veya `x86` ise, **`x64`** olarak deðiþtirin.
5.  Ayarlarý kaydedin (`Ctrl + S`) ve projeyi yeniden baþlatýn (Rebuild Solution).

Eðer `Any CPU` kullanmak zorundaysanýz, **"Prefer 32-bit" (32-bit tercih et)** seçeneðinin **iþaretini kaldýrdýðýnýzdan** emin olun. Bu, projenin 64-bit bir sistemde 64-bit olarak çalýþmasýný saðlar.

## Kullanýlan Teknolojiler

* **C# / .NET Framework**
* **Windows Forms:** Masaüstü arayüzü
* **Vosk:** Çevrimdýþý ses tanýma motoru
* **NAudio:** Mikrofondan ses verisi alma