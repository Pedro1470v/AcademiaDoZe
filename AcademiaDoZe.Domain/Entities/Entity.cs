//Pedro Vinicius
using System;

namespace AcademiaDoZe.Domain.Entities
{
    internal abstract class Entidade
    {
        public int ID { get; }

        protected Entidade(int id = 0)
        {
            if (id < 0)
                throw new Exception("O ID não pode ser negativo.");

            ID = id;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Entidade outra)
                return false;

            if (ReferenceEquals(this, outra))
                return true;

            return ID == outra.ID && GetType() == outra.GetType();
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + ID).GetHashCode();
        }
    }
}