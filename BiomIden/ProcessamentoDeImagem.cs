using System;
using System.Drawing;

namespace BiomIden
{
    public static class ProcessamentoDeImagem
    {
        public static Bitmap ConverterParaEscalaDeCinza(Bitmap imagem)
        {
            Bitmap imagemCinza = new Bitmap(imagem.Width, imagem.Height);
            for (int y = 0; y < imagem.Height; y++)
            {
                for (int x = 0; x < imagem.Width; x++)
                {
                    Color corPixel = imagem.GetPixel(x, y);
                    int intensidade = (int)(0.3 * corPixel.R + 0.59 * corPixel.G + 0.11 * corPixel.B);
                    Color corCinza = Color.FromArgb(intensidade, intensidade, intensidade);
                    imagemCinza.SetPixel(x, y, corCinza);
                }
            }
            return imagemCinza;
        }

        public static Bitmap AjustarBrilhoEContraste(Bitmap imagem, double fatorContraste, int fatorBrilho)
        {
            Bitmap imagemAjustada = new Bitmap(imagem.Width, imagem.Height);
            for (int y = 0; y < imagem.Height; y++)
            {
                for (int x = 0; x < imagem.Width; x++)
                {
                    Color corPixel = imagem.GetPixel(x, y);
                    int novaR = (int)(fatorContraste * (corPixel.R - 128) + 128 + fatorBrilho);
                    int novaG = (int)(fatorContraste * (corPixel.G - 128) + 128 + fatorBrilho);
                    int novaB = (int)(fatorContraste * (corPixel.B - 128) + 128 + fatorBrilho);
                    novaR = Math.Min(255, Math.Max(0, novaR));
                    novaG = Math.Min(255, Math.Max(0, novaG));
                    novaB = Math.Min(255, Math.Max(0, novaB));
                    Color novaCor = Color.FromArgb(novaR, novaG, novaB);
                    imagemAjustada.SetPixel(x, y, novaCor);
                }
            }
            return imagemAjustada;
        }

        public static int[,] FiltroSobel(int[,] imagemCinza)
        {
            int largura = imagemCinza.GetLength(0);
            int altura = imagemCinza.GetLength(1);
            int[,] imagemBordas = new int[largura, altura];

            int[,] sobelX = new int[,]
            {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            int[,] sobelY = new int[,]
            {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };

            for (int y = 1; y < altura - 1; y++)
            {
                for (int x = 1; x < largura - 1; x++)
                {
                    int gx = 0;
                    int gy = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            gx += imagemCinza[x + i, y + j] * sobelX[i + 1, j + 1];
                            gy += imagemCinza[x + i, y + j] * sobelY[i + 1, j + 1];
                        }
                    }

                    int magnitude = (int)Math.Sqrt((gx * gx) + (gy * gy));
                    imagemBordas[x, y] = Math.Min(255, Math.Max(0, magnitude));
                }
            }

            return imagemBordas;
        }

        public static Bitmap EqualizarHistograma(Bitmap imagem)
        {
            int[] histograma = new int[256];
            Bitmap imagemEqualizada = new Bitmap(imagem.Width, imagem.Height);

            for (int y = 0; y < imagem.Height; y++)
            {
                for (int x = 0; x < imagem.Width; x++)
                {
                    Color corPixel = imagem.GetPixel(x, y);
                    int intensidade = corPixel.R;
                    histograma[intensidade]++;
                }
            }

            int totalPixels = imagem.Width * imagem.Height;
            int[] distribucaoAcumulada = new int[256];
            distribucaoAcumulada[0] = histograma[0];

            for (int i = 1; i < 256; i++)
            {
                distribucaoAcumulada[i] = distribucaoAcumulada[i - 1] + histograma[i];
            }

            int[] novaIntensidade = new int[256];
            for (int i = 0; i < 256; i++)
            {
                novaIntensidade[i] = (int)((double)(distribucaoAcumulada[i] * 255) / totalPixels);
            }

            for (int y = 0; y < imagem.Height; y++)
            {
                for (int x = 0; x < imagem.Width; x++)
                {
                    Color corPixel = imagem.GetPixel(x, y);
                    int intensidade = corPixel.R;
                    int novaInt = novaIntensidade[intensidade];
                    Color corNova = Color.FromArgb(novaInt, novaInt, novaInt);
                    imagemEqualizada.SetPixel(x, y, corNova);
                }
            }

            return imagemEqualizada;
        }
    }
}
