using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMessaging
{
    void OnNonIntrusiveMessage(NonIntrusiveMessage message);
    object OnIntrusiveMessage(IntrusiveMessage message);
}
