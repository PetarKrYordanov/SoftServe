using System;
using System.Collections.Generic;
using System.Text;

namespace ImageServe.Models.Common
{
    public class BaseModel<T>
    {
        public T Id { get; set; }
    }
}
