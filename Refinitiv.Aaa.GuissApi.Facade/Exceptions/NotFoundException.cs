using System;

namespace Refinitiv.Aaa.GuissApi.Facade.Exceptions
{
    /// <summary>
    /// Exception indicating that user or attribute is not found.
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException(string message):base(message)
        {
        }
    }
}
