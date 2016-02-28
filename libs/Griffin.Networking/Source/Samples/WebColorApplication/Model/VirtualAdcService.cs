using System;

namespace WebColorApplication.Model
{
    public class VirtualAdcService : IAdcService
    {
        public int Read(int id)
        {
            return 100;
        }

        public event EventHandler<int> ValueChanged;
    }
}
