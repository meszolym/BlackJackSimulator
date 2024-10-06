using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator.Models.Errors
{
    public record HandError(Option<string> Message, Option<Exception> InnerException);
}
