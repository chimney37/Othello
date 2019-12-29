using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    public static class OthelloExceptions
    {
        public static void ThrowExceptionIfNull(Object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
        }
    }
}
