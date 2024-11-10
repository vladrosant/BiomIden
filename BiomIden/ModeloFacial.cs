using System;
using System.IO;
using Newtonsoft.Json;

namespace BiomIden
{
    public class ModeloFacial
    {
        public int IntensidadeRegiaoOlho { get; set; }
        public int IntensidadeRegiaoBoca { get; set; }
        public string UserId { get; set; }

        public ModeloFacial(int regiaoOlho, int regiaoBoca, string userId)
        {
            IntensidadeRegiaoOlho = regiaoOlho;
            IntensidadeRegiaoBoca = regiaoBoca;
            UserId = userId;
        }

        public static (int regiaoOlho, int regiaoBoca) ExtraiCaracteristicasFaciais(int[,] imagemBordas)
        {
            int width = imagemBordas.GetLength(0);
            int height = imagemBordas.GetLength(1);

            int regiaoOlho = 0;
            int regiaoBoca = 0;

            for (int y = 0; y < height / 2; y++)
            {
                for (int x = width / 4; x < 3 * width / 4; x++)
                {
                    if (imagemBordas[x, y] > 128)
                    {
                        regiaoOlho++;
                    }
                }
            }

            for (int y = height / 2; y < height; y++)
            {
                for (int x = width / 4; x < 3 * width / 4; x++)
                {
                    if (imagemBordas[x, y] > 128)
                    {
                        regiaoBoca++;
                    }
                }
            }
            return (regiaoOlho, regiaoBoca);
        }

        public static ModeloFacial CriaModeloFacial(int[,] imagemReferencia, string userId)
        {
            int[,] imagemBordas = ProcessamentoDeImagem.FiltroSobel(imagemReferencia);
            var (regiaoOlho, regiaoBoca) = ExtraiCaracteristicasFaciais(imagemBordas);

            ModeloFacial modelo = new ModeloFacial(regiaoOlho, regiaoBoca, userId);
            GravaArquivoModelo(modelo, $"modelo_{userId}.json");
            return modelo;
        }

        public static void GravaArquivoModelo(ModeloFacial modelo, string caminhoArquivo)
        {
            string json = JsonConvert.SerializeObject(modelo, Formatting.Indented);
            File.WriteAllText(caminhoArquivo, json);
        }
    }
}