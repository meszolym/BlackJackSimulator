using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace BlackJackSimulator.Models.Errors
{
    public record ShoeError(Option<string> Message, Option<Exception> InnerException);
}
