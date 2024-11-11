using System;
using System.Drawing;
using System.IO;

namespace BiomIden
{
    class Program
    {
        static void Main(string[] args)
        {
            // Verificar se o número correto de argumentos foi fornecido
            if (args.Length != 2)
            {
                Console.WriteLine("Uso: BiomIden <UsuarioID> <CaminhoImagem>");
                return;
            }

            string usuarioId = args[0];
            string caminhoImagem = args[1];

            // Verificar se o arquivo existe
            if (!File.Exists(caminhoImagem))
            {
                Console.WriteLine("Erro: Arquivo de imagem não encontrado.");
                return;
            }

            // Carregar e processar a imagem de entrada
            Bitmap imagemEntrada = new Bitmap(caminhoImagem);
            Bitmap imagemCinza = ProcessamentoDeImagem.ConverterParaEscalaDeCinza(imagemEntrada);
            Bitmap imagemEqualizada = ProcessamentoDeImagem.EqualizarHistograma(imagemCinza);
            Bitmap imagemAjustada = ProcessamentoDeImagem.AjustarBrilhoEContraste(imagemEqualizada, 1.2, 10);

            int[,] imagemArray = BitmapParaArray(imagemAjustada);

            // Carregar o modelo facial salvo do usuário
            string caminhoModelo = $"E:\\VamPipo\\repos\\BiomIden\\BiomIden\\modelo_{usuarioId}.json";
            if (!File.Exists(caminhoModelo))
            {
                Console.WriteLine("Erro: Modelo do usuário não encontrado.");
                return;
            }

            ModeloFacial modeloSalvo = ModeloFacial.CarregaModelo(caminhoModelo);

            // Autenticar o usuário com base na imagem de entrada e no modelo salvo
            bool autenticado = AutenticarUsuario(imagemArray, modeloSalvo);

            if (autenticado)
            {
                Console.WriteLine($"Usuário {usuarioId} autenticado com sucesso.");
            }
            else
            {
                Console.WriteLine($"Falha na autenticação para o usuário {usuarioId}.");
            }
        }

        static int[,] BitmapParaArray(Bitmap bitmap)
        {
            int largura = bitmap.Width;
            int altura = bitmap.Height;
            int[,] array = new int[largura, altura];

            for (int y = 0; y < altura; y++)
            {
                for (int x = 0; x < largura; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    array[x, y] = pixel.R;
                }
            }
            return array;
        }

        // Função de autenticação para comparar o modelo salvo com as características da imagem de entrada
        static bool AutenticarUsuario(int[,] imagemEntrada, ModeloFacial modeloSalvo)
        {
            int[,] imagemBordas = ProcessamentoDeImagem.FiltroSobel(imagemEntrada);
            var (regiaoOlho, regiaoBoca) = ModeloFacial.ExtraiCaracteristicasFaciais(imagemBordas);

            int tolerancia = 50;
            bool olhosCorresponde = Math.Abs(regiaoOlho - modeloSalvo.IntensidadeRegiaoOlho) < tolerancia;
            bool bocaCorresponde = Math.Abs(regiaoBoca - modeloSalvo.IntensidadeRegiaoBoca) < tolerancia;

            return olhosCorresponde && bocaCorresponde;
        }
    }
}
