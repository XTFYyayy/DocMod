using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;
using System;
using System.Collections.Generic;

namespace Doc.DocCode.PotionPools;

public sealed class DocPotionPool : PotionPoolModel
{
    public override string EnergyColorName => "doc";
    public override Color LabOutlineColor => new Color("c4278a");

    protected override IEnumerable<PotionModel> GenerateAllPotions()
    {
        // 博士专用药水列表
        // 目前为空，后续添加博士专属药水时在这里加入
        return Array.Empty<PotionModel>();
    }

    public override IEnumerable<PotionModel> GetUnlockedPotions(UnlockState unlockState)
    {
        // 如果需要解锁条件，可以在这里添加
        // 暂时返回所有药水
        return GenerateAllPotions();
    }
}