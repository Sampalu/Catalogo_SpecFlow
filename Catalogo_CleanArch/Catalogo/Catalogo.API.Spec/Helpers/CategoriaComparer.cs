using Catalogo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalogo.API.Spec.Helpers
{
    public class CategoriaComparer : IEqualityComparer<Categoria>
    {
        public bool Equals(Categoria? x, Categoria? y)
        {
            //if (ReferenceEquals(x, y)) return true;
            //if (ReferenceEquals(x, null)) return false;
            //if (ReferenceEquals(y, null)) return false;
            //if (x.GetType() != y.GetType()) return false;
            return x.CategoriaId == y.CategoriaId && x.Nome == y.Nome && x.ImagemUrl == y.ImagemUrl;
        }

        public int GetHashCode(Categoria obj)
        {
            return HashCode.Combine(obj.CategoriaId, obj.Nome, obj.ImagemUrl);
        }
    }
}
