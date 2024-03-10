using Domain.FichaDomino;

namespace Application.FichaDomino
{
    public class FichaDominoApplication
    {
        /// <summary>
        /// Metodo para convertir la data recibida.
        /// </summary>
        /// <param name="dataFichas"></param>
        /// <returns>Retorna una lista de Fichas de Domino.</returns>
        public List<FichaDominoEntity> SerializerListaFichasDomino(string? dataFichas)
        {
            string[] fichasDomino = dataFichas.Replace("[", "").Split("]");
            List<FichaDominoEntity> listaFichaDomino = new();

            foreach (string fichaString in fichasDomino.SkipLast(1).ToArray())
            {
                string[] ficha = fichaString.Split("|");
                FichaDominoEntity fichaDomino = new()
                {
                    Izquierda = int.Parse(ficha[0]),
                    Derecha = int.Parse(ficha[1])
                };
                listaFichaDomino.Add(fichaDomino);
            }

            return listaFichaDomino;
        }

        /// <summary>
        /// Metodo para ordenar una lista de Fichas de Domino.
        /// </summary>
        /// <param name="listaFichasDomino"></param>
        /// <returns>Devuelve una lista de Fichas de Domino Ordenada.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public List<FichaDominoEntity> Ordenar(List<FichaDominoEntity>? listaFichasDomino)
        {
            if (listaFichasDomino == null || !listaFichasDomino.Any()) return new List<FichaDominoEntity>();

            var sortedList = new List<FichaDominoEntity> { listaFichasDomino.First() };
            listaFichasDomino.RemoveAt(0);

            while (listaFichasDomino.Any())
            {
                bool foundMatch = false;
                for (int i = 0; i < listaFichasDomino.Count && !foundMatch; i++)
                {
                    var piece = listaFichasDomino[i];
                    if (sortedList.Last().Derecha == piece.Izquierda)
                    {
                        sortedList.Add(piece);
                        listaFichasDomino.RemoveAt(i);
                        foundMatch = true;
                    }
                    else if (sortedList.Last().Derecha == piece.Derecha)
                    {
                        piece = Voltear(piece);
                        sortedList.Add(piece);
                        listaFichasDomino.RemoveAt(i);
                        foundMatch = true;
                    }
                }
                if (!foundMatch) throw new InvalidOperationException("No se puede formar una secuencia continua con las fichas proporcionadas.");
            }
            return sortedList;
        }

        /// <summary>
        /// Metodo para voltear una Ficha de Domino.
        /// </summary>
        /// <param name="dichaDominoEntity"></param>
        /// <returns>Devuelve una Ficha de Domino girada.</returns>
        private FichaDominoEntity Voltear(FichaDominoEntity dichaDominoEntity)
        {
            (dichaDominoEntity.Izquierda, dichaDominoEntity.Derecha) = (dichaDominoEntity.Derecha, dichaDominoEntity.Izquierda);

            return dichaDominoEntity;
        }

        /// <summary>
        /// Metodo para generar una cadena de texto de las Fichas de Domino Ordenadas.
        /// </summary>
        /// <param name="listaFichaDominoOrdenado"></param>
        /// <returns>Devuelve una cadena de texto de las Fichas Ordenadas. Ejemplo "[2|1][2|3][1|3]".</returns>
        public string FormatearFichasDomino(List<FichaDominoEntity> listaFichaDominoOrdenado)
        {
            var fichasResultado = "";
            foreach (var domino in listaFichaDominoOrdenado)
            {
                fichasResultado += String.Format("[{0}|{1}]", domino.Izquierda, domino.Derecha);
            }

            return fichasResultado;
        }
    }
}
