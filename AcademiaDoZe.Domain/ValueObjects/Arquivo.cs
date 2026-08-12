//Pedro Vinicius
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    internal class Arquivo
    {
        public byte[] Conteudo { get; }
        public string Tipo { get; }

        private Arquivo(byte[] conteudo)
        {
            Conteudo = conteudo;
            Tipo = "Arquivo";
        }

        public static Result<Arquivo> Criar(byte[] conteudo)
        {
            var notifications = new List<Notification>();

            if (conteudo == null || conteudo.Length == 0)
            {
                notifications.Add(
                    new Notification(
                        "Conteudo",
                        "ARQUIVO_VAZIO"));
            }

            const int tamanhoMaximoBytes = 5 * 1024 * 1024;

            if (conteudo != null && conteudo.Length > tamanhoMaximoBytes)
            {
                notifications.Add(
                    new Notification(
                        "Conteudo",
                        "ARQUIVO_MAIOR_QUE_5_MB"));
            }

            if (notifications.Count != 0)
            {
                return Result<Arquivo>.Failure(notifications);
            }

            return Result<Arquivo>.Success(
                new Arquivo(conteudo!));
        }
    }
}