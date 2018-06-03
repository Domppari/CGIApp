using System.Collections.Generic;

namespace BusinessIdSpecification
{
    public class Error<T>
    {
        private Dictionary<T, string> errorCatalog = new Dictionary<T, string>();
        private List<T> errors = new List<T>();

        public void InitError(T error, string text)
        {
            errorCatalog.Add(error, text);
        }

        public void ReportError(T error)
        {
            errors.Add(error);
        }

        public IEnumerable<string> GetReportedErrors()
        {
            var errorList = new List<string>();            
            for (var i = 0; i < errors.Count; i++)
                errorList.Add(errorCatalog[errors[i]]);

            return errorList;
        }
        
        public void ClearReportedErrors()
        {
            errors.Clear();
        }   
    }
}
