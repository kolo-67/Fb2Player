using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fb2PlayerCommon.Contracts
{
    public interface IPlayerActionQueriable
    {
        event Action PlayQuery;
        event Action PauseQuery;
        event Action StopQuery;
        event Action<object, object> ListChangeQuery;
        event Action<object> FolderDialogQuery;
        event Action<object> ChangePathDialogQuery;
        void EndAction(bool isFailed);
        int TrackByList(object pList);
        void OnLoad();
    }
}
