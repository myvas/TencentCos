using System.Collections.Generic;

namespace Myvas.AspNetCore.TencentCos
{
    public class MultiObjectDeleteException : CosServiceException
    {
        public List<DeleteError> Errors { get; set; } = new List<DeleteError>();
        public List<DeletedObject> DeletedObjects { get; set; } = new List<DeletedObject>();
    }
}
