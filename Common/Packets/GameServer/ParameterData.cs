using System;

namespace SagaBNS.Common.Packets.GameServer
{
    public class ParameterData : Attribute
    {
        internal ParameterData(int length)
        {
            Length = length;
        }

        public int Length { get; private set; }
    }
}
