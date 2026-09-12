using UnityEngine;
using ZXing;
using ZXing.Common;

public class QrCodeScanner : MonoBehaviour
{
    private WebCamTexture cameraTexture;
    private BarcodeReaderGeneric barcodeReader;
    public QRChallengeManager challengeManager;

    private void Start()
    {
        // Cria o leitor de QR Code.
        barcodeReader = new BarcodeReaderGeneric
        {
            AutoRotate = true,
            Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new[]
                {
                    BarcodeFormat.QR_CODE
                }
            }
        };

        // Cria a captura da câmera.
        cameraTexture = new WebCamTexture();

        // Inicia a câmera.
        cameraTexture.Play();

        Debug.Log("Scanner de QR Code iniciado.");
    }

    private void Update()
    {
        if (cameraTexture == null || !cameraTexture.isPlaying)
            return;

        // Aguarda a câmera começar a fornecer imagens.
        if (cameraTexture.width < 100)
            return;

        // Obtém os pixels da câmera.
        Color32[] pixels = cameraTexture.GetPixels32();

        // Converte Color32[] para byte[] no formato RGB.
        byte[] rawData = new byte[pixels.Length * 3];

        for (int i = 0; i < pixels.Length; i++)
        {
            rawData[i * 3] = pixels[i].r;
            rawData[i * 3 + 1] = pixels[i].g;
            rawData[i * 3 + 2] = pixels[i].b;
        }

        // Tenta encontrar um QR Code.
        var result = barcodeReader.Decode(
            rawData,
            cameraTexture.width,
            cameraTexture.height,
            RGBLuminanceSource.BitmapFormat.RGB24
        );

        if (result != null)
        {
            Debug.Log($"QR Code encontrado: {result.Text}");
            
            if (challengeManager != null)
            {
                challengeManager.ProcessQRCode(result.Text);
            }
        }

    }

    private void OnDestroy()
    {
        if (cameraTexture != null && cameraTexture.isPlaying)
        {
            cameraTexture.Stop();
        }
    }
}