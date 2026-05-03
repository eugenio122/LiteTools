using System.Collections.Concurrent;
using LiteTools.Interfaces;

namespace LiteTools.Core
{
    /// <summary>
    /// Gerenciador de estado global e memória da Nave-Mãe.
    /// Permite que plugins diferentes compartilhem informações de contexto de forma desacoplada
    /// (ex: o LiteFlow salva o "Passo Atual", e o LiteJson consulta essa informação sem conhecer o LiteFlow).
    /// </summary>
    public class LiteHostContext : ILiteHostContext
    {
        /// <summary>
        /// Dicionário thread-safe para armazenar os metadados.
        /// O uso do ConcurrentDictionary é obrigatório aqui, pois plugins diferentes podem 
        /// tentar ler e gravar dados simultaneamente a partir de threads distintas (Background vs UI).
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _metadata = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Salva ou atualiza um valor no estado global da sessão.
        /// </summary>
        /// <param name="key">A chave identificadora (ex: "CurrentTestCaseName")</param>
        /// <param name="value">O objeto a ser armazenado</param>
        public void SetSessionMetadata(string key, object value)
        {
            _metadata[key] = value;
        }

        /// <summary>
        /// Recupera um valor do estado global da sessão no formato de objeto puro.
        /// </summary>
        /// <param name="key">A chave identificadora do dado desejado</param>
        /// <returns>O objeto correspondente, ou nulo caso a chave não exista.</returns>
        public object GetSessionMetadata(string key)
        {
            _metadata.TryGetValue(key, out var value);
            return value;
        }

        /// <summary>
        /// Tenta recuperar um valor do estado global já convertendo (cast) para o tipo especificado.
        /// Esta é a forma mais segura de ler dados no ecossistema.
        /// </summary>
        /// <typeparam name="T">O tipo esperado do dado (ex: string, bool, int)</typeparam>
        /// <param name="key">A chave identificadora do dado desejado</param>
        /// <param name="value">A variável de saída que receberá o valor convertido</param>
        /// <returns>Verdadeiro se a chave existir e o tipo for compatível; caso contrário, falso.</returns>
        public bool TryGetSessionMetadata<T>(string key, out T value)
        {
            if (_metadata.TryGetValue(key, out var rawValue) && rawValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }
    }
}