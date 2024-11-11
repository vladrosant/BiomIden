using System;
using System.IO;
using Newtonsoft.Json;

namespace BiomIden
{
    public class ModeloFacial
    {
        public int IntensidadeRegiaoOlho { get; set; }
        public int IntensidadeRegiaoBoca { get; set; }
        public string UsuarioId { get; set; }

        public ModeloFacial(int regiaoOlho, int regiaoBoca, string usuarioId)
        {
            IntensidadeRegiaoOlho = regiaoOlho;
            IntensidadeRegiaoBoca = regiaoBoca;
            UsuarioId = usuarioId;
        }

        public static (int regiaoOlho, int regiaoBoca) ExtraiCaracteristicasFaciais(int[,] imagemBordas)
        {
            int largura = imagemBordas.GetLength(0);
            int altura = imagemBordas.GetLength(1);

            int regiaoOlho = 0;
            int regiaoBoca = 0;

            for (int y = 0; y < altura / 2; y++)
            {
                for (int x = largura / 4; x < 3 * largura / 4; x++)
                {
                    if (imagemBordas[x, y] > 128)
                    {
                        regiaoOlho++;
                    }
                }
            }

            for (int y = altura / 2; y < altura; y++)
            {
                for (int x = largura / 4; x < 3 * largura / 4; x++)
                {
                    if (imagemBordas[x, y] > 128)
                    {
                        regiaoBoca++;
                    }
                }
            }
            return (regiaoOlho, regiaoBoca);
        }

        public static ModeloFacial CriaModeloFacial(int[,] imagemReferencia, string usuarioId)
        {
            int[,] imagemBordas = ProcessamentoDeImagem.FiltroSobel(imagemReferencia);
            var (regiaoOlho, regiaoBoca) = ExtraiCaracteristicasFaciais(imagemBordas);

            ModeloFacial modelo = new ModeloFacial(regiaoOlho, regiaoBoca, usuarioId);
            GravaArquivoModelo(modelo, $"E:\\VamPipo\\repos\\BiomIden\\BiomIden\\modelo_{usuarioId}.json");
            return modelo;
        }

        public static void GravaArquivoModelo(ModeloFacial modelo, string caminhoArquivo)
        {
            string json = JsonConvert.SerializeObject(modelo, Formatting.Indented);
            File.WriteAllText(caminhoArquivo, json);
        }

        public static ModeloFacial CarregaModelo(string caminhoArquivo)
        {
            string json = File.ReadAllText(caminhoArquivo);
            return JsonConvert.DeserializeObject<ModeloFacial>(json);
        }
    }
}
