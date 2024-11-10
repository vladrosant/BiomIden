using System;
using System.Drawing;

namespace BiomIden
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example usage
            Bitmap imagem = new Bitmap("path_to_image.jpg");
            Bitmap imagemCinza = ProcessamentoDeImagem.ConverterParaEscalaDeCinza(imagem);
            Bitmap imagemEqualizada = ProcessamentoDeImagem.EqualizarHistograma(imagemCinza);
            Bitmap imagemAjustada = ProcessamentoDeImagem.AjustarBrilhoEContraste(imagemEqualizada, 1.2, 10);

            int[,] imagemCinzaArray = BitmapToArray(imagemAjustada);
            ModeloFacial modelo = ModeloFacial.CriaModeloFacial(imagemCinzaArray, "user123");

            Console.WriteLine("Modelo facial criado e salvo com sucesso.");
        }

        static int[,] BitmapToArray(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[,] array = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    array[x, y] = pixel.R;
                }
            }
            return array;
        }
    }
}