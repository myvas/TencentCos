using System;
using System.Collections.Generic;
using System.Text;
using AspNetCore.TencentCos.Models;

namespace AspNetCore.TencentCos.Exceptions
{
    public class MultiObjectDeleteException : CosServiceException
    {
        public List<DeleteError> Errors { get; set; } = new List<DeleteError>();
        public List<DeletedObject> DeletedObjects { get; set; } = new List<DeletedObject>();
    }
}
