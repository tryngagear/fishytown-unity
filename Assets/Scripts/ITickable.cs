using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITickable
{
    void Tick(int tick);
    void LongTick(int tick);
}
