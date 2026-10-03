using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using ZXing.Common;

public class QrCodeScanner : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] private QRChallengeManager challengeManager;

    [Header("Scanner")]
    [SerializeField] private float scanInterval = 0.3f;

    // Define quando o próximo frame poderá ser analisado.
    // Isso evita tentar processar todos os frames da câmera.
    private float nextScanTime;

    // Impede que o mesmo QR Code seja processado
    // repetidamente após ser encontrado.
    private bool qrDetected = false;

    // Controla se o scanner está autorizado
    // a processar os frames recebidos da câmera.
    private bool scannerEnabled = false;

    /*
     * Leitor da biblioteca ZXing responsável por
     * interpretar os pixels da câmera como QR Code.
     *
     * AutoRotate permite reconhecer códigos em diferentes orientações.
     * TryHarder aumenta o esforço de processamento para melhorar
     * a chance de detectar códigos mais difíceis.
     *
     * PossibleFormats restringe a busca apenas a QR Codes,
     * evitando procurar outros tipos de códigos de barras.
     */
    private readonly BarcodeReaderGeneric barcodeReader =
        new BarcodeReaderGeneric
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

    private void OnEnable()
    {
        /*
         * Quando este componente é habilitado,
         * o método OnCameraFrameReceived passa a ser chamado
         * sempre que o ARCameraManager disponibilizar um novo frame.
         */
        if (cameraManager != null)
        {
            cameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    private void OnDisable()
    {
        /*
         * Remove a inscrição no evento quando o componente
         * é desabilitado, evitando chamadas desnecessárias
         * e referências ao objeto após ele deixar de estar ativo.
         */
        if (cameraManager != null)
        {
            cameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    /// <summary>
    /// Ativa o processamento de QR Codes e permite
    /// que um novo código seja reconhecido.
    /// </summary>
    public void EnableScanner()
    {
        scannerEnabled = true;
        qrDetected = false;

        // Permite que a primeira tentativa de leitura
        // seja feita imediatamente.
        nextScanTime = Time.time;

        Debug.Log("Scanner de QR Code ativado.");
    }

    /// <summary>
    /// Interrompe temporariamente o processamento
    /// dos frames da câmera.
    /// </summary>
    public void DisableScanner()
    {
        scannerEnabled = false;
        qrDetected = false;

        Debug.Log("Scanner de QR Code desativado.");
    }

    /// <summary>
    /// Prepara o scanner para procurar um novo QR Code,
    /// normalmente após o término de um desafio.
    /// </summary>
    public void ResetScanner()
    {
        scannerEnabled = true;
        qrDetected = false;
        nextScanTime = Time.time;

        Debug.Log("Scanner de QR Code reativado.");
    }

    /// <summary>
    /// Processa os frames fornecidos pela câmera AR
    /// e tenta encontrar um QR Code.
    /// </summary>
    private void OnCameraFrameReceived(
        ARCameraFrameEventArgs args)
    {
        // Não processa imagens enquanto o scanner
        // estiver desativado pelo fluxo da aplicação.
        if (!scannerEnabled)
        {
            return;
        }

        // Depois que um QR já foi encontrado,
        // os frames deixam de ser analisados até o scanner ser resetado.
        if (qrDetected)
        {
            return;
        }

        /*
         * Limita a frequência das leituras.
         * O scanner espera scanInterval segundos
         * entre cada tentativa de reconhecer um QR Code.
         */
        if (Time.time < nextScanTime)
        {
            return;
        }

        nextScanTime = Time.time + scanInterval;

        /*
         * Obtém a imagem mais recente da câmera em formato
         * XRCpuImage, permitindo que seus pixels sejam
         * processados diretamente pela CPU.
         */
        if (!cameraManager.TryAcquireLatestCpuImage(
            out XRCpuImage image))
        {
            return;
        }

        try
        {
            /*
             * Limita a largura utilizada no processamento.
             * Imagens muito grandes aumentariam desnecessariamente
             * o custo da leitura do QR Code.
             */
            int targetWidth = 1280;

            /*
             * Nunca aumenta a resolução da imagem.
             * Caso ela seja maior que 1280 pixels de largura,
             * calcula um fator para reduzi-la proporcionalmente.
             */
            float scale = Mathf.Min(
                1f,
                targetWidth / (float)image.width
            );

            int width = Mathf.RoundToInt(
                image.width * scale
            );

            int height = Mathf.RoundToInt(
                image.height * scale
            );

            /*
             * Define como a imagem capturada pelo ARCore
             * será convertida para um formato utilizável pelo ZXing.
             */
            var conversionParams =
                new XRCpuImage.ConversionParams
                {
                    // Utiliza toda a área da imagem original.
                    inputRect = new RectInt(
                        0,
                        0,
                        image.width,
                        image.height
                    ),

                    // Define a resolução final após o redimensionamento.
                    outputDimensions = new Vector2Int(
                        width,
                        height
                    ),

                    /*
                     * Converte cada pixel para RGB com
                     * três canais de 8 bits.
                     */
                    outputFormat = TextureFormat.RGB24,

                    // Mantém a orientação original da imagem.
                    transformation =
                        XRCpuImage.Transformation.None
                };

            /*
             * Calcula quantos bytes serão necessários
             * para armazenar a imagem convertida.
             */
            int size =
                image.GetConvertedDataSize(
                    conversionParams
                );

            /*
             * Cria um buffer temporário de memória nativa
             * para receber os pixels convertidos.
             *
             * Allocator.Temp é utilizado porque os dados
             * só são necessários durante este processamento.
             */
            using var buffer =
                new NativeArray<byte>(
                    size,
                    Allocator.Temp
                );

            // Converte a imagem da câmera utilizando
            // as configurações definidas anteriormente.
            image.Convert(
                conversionParams,
                buffer
            );

            /*
             * Converte o NativeArray para byte[],
             * formato esperado pelo leitor ZXing.
             */
            byte[] pixels = buffer.ToArray();

            /*
             * O ZXing analisa os pixels RGB da imagem
             * tentando localizar e decodificar um QR Code.
             */
            Result result = barcodeReader.Decode(
                pixels,
                width,
                height,
                RGBLuminanceSource.BitmapFormat.RGB24
            );

            // Caso um QR Code tenha sido encontrado,
            // envia o texto decodificado para processamento.
            if (result != null)
            {
                ProcessQRCode(result.Text);
            }
        }
        catch (Exception e)
        {
            // Evita que um problema durante a conversão
            // ou leitura da imagem interrompa a aplicação.
            Debug.LogWarning(
                $"Erro ao processar imagem da câmera: {e.Message}"
            );
        }
        finally
        {
            /*
             * XRCpuImage utiliza recursos nativos.
             * Dispose libera esses recursos mesmo caso
             * ocorra uma exceção durante o processamento.
             */
            image.Dispose();
        }
    }

    /// <summary>
    /// Valida o conteúdo encontrado e encaminha
    /// o identificador para o gerenciador de desafios.
    /// </summary>
    private void ProcessQRCode(string qrContent)
    {
        // Ignora resultados sem conteúdo válido.
        if (string.IsNullOrWhiteSpace(qrContent))
        {
            return;
        }

        /*
         * Bloqueia novas leituras enquanto o desafio atual
         * estiver sendo processado.
         */
        qrDetected = true;

        Debug.Log(
            $"QR Code detectado: {qrContent}"
        );

        /*
         * O scanner é responsável apenas pela leitura.
         * A decisão de qual desafio abrir é delegada
         * ao QRChallengeManager.
         */
        if (challengeManager != null)
        {
            challengeManager.ProcessQRCode(
                qrContent
            );
        }
    }
}