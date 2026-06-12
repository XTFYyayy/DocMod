using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace Doc.DocCode.Monsters;

public sealed class BlazingSun : MonsterModel
{
    public const string Id = "BlazingSun";

    public override bool IsPet => true;

    // 【为你而死】能力由 DieForYouPower 提供，在召唤时自动添加
}