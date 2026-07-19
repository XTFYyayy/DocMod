using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers
{
    internal class StunPower:CustomPowerModel
    {
        //这是一个空方法，仅用于添加卡牌中击晕的Tips描述
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override string? CustomPackedIconPath => "stun_power.png".PowerImagePath();
        public override string? CustomBigIconPath => "stun_power.png".PowerImagePath();
    }
}
