using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fb2PlayerCommon.Contracts
{
    public interface IScrollIntoViewAction
    {
        event Action<object> MainGridScrollIntoView;
        event Action SpeechPhraseScrollIntoView;
    }
}
 