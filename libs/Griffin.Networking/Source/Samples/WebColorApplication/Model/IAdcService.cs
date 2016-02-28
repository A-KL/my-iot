using System;

namespace WebColorApplication.Model
{
    public interface IAdcService
    {
        int Read(int id);

        event EventHandler<int> ValueChanged;
    }
}